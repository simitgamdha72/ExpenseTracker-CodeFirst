using System.Net;
using ExpenseTracker.Models.Dto;
using ExpenseTracker.Models.Models;
using ExpenseTracker.Models.Validations.Constants.ErrorMessages;
using ExpenseTracker.Models.Validations.Constants.SuccessMessages;
using ExpenseTracker.Repository.Interface;
using ExpenseTracker.Service.Interface;
using ExpenseTracker.Service.Validations;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Service.Implementation;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, JwtService jwt, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<Response<object?>> LoginUserAsync(LoginRequestDto loginRequestDto)
    {
        _logger.LogInformation("Attempting login for email: {Email}", loginRequestDto.Email);

        // Check if user exists
        User? user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);

        // Invalid credentials
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed for email: {Email} - Invalid credentials", loginRequestDto.Email);

            return ResponseHelper.Error(
                message: ErrorMessages.InvalidCredentials,
                statusCode: HttpStatusCode.BadRequest
            );
        }

        // Generate JWT token
        string token = _jwt.GenerateToken(user);

        _logger.LogInformation("Login successful for email: {Email}", loginRequestDto.Email);

        return ResponseHelper.Success<object?>(
         data: token,
         message: SuccessMessages.LoginSuccessful
     );

    }

    public async Task<Response<object?>> RegisterUserAsync(RegisterRequestDto registerRequestDto)
    {
        _logger.LogInformation("Registration attempt for email: {Email}, username: {Username}", registerRequestDto.Email, registerRequestDto.Username);

        // Role Validation
        if (registerRequestDto.RoleId != 1 && registerRequestDto.RoleId != 2)
        {
            _logger.LogWarning("Registration failed: Invalid role ({RoleId}) for email: {Email}", registerRequestDto.RoleId, registerRequestDto.Email);

            return ResponseHelper.Error(
                message: ErrorMessages.InvalidRole,
                statusCode: HttpStatusCode.BadRequest
            );
        }

        // Check if user exists
        bool exists = await _userRepository.EmailOrUsernameExistsAsync(registerRequestDto.Email, registerRequestDto.Username);
        if (exists)
        {
            _logger.LogWarning("Registration failed: Email or username already exists - Email: {Email}, Username: {Username}", registerRequestDto.Email, registerRequestDto.Username);

            return ResponseHelper.Error(
             message: ErrorMessages.EmailOrUsernameExists,
             statusCode: HttpStatusCode.Conflict
            );
        }

        // Create user
        User? user = new User
        {
            Username = registerRequestDto.Username,
            Email = registerRequestDto.Email,
            Firstname = registerRequestDto.Firstname,
            Lastname = registerRequestDto.Lastname,
            Contactnumber = registerRequestDto.Contactnumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password),
            RoleId = registerRequestDto.RoleId ?? 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Addres = registerRequestDto.Address,
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User registered successfully - Email: {Email}, Username: {Username}", user.Email, user.Username);

        // Return successful response
        RegisterRequestDto? data = new RegisterRequestDto
        {
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Contactnumber = user.Contactnumber,
            Address = user.Addres
        };

        return ResponseHelper.Success<object?>(data, SuccessMessages.UserRegistered);
    }

}
