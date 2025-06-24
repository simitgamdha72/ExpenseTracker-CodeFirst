using System.Net;
using ExpenseTracker.Models.Dto;

namespace ExpenseTracker.Service;

public static class ResponseHelper
{
    public static Response<T> Success<T>(T? data = default, string? message = "Success", HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new Response<T>
        {
            Succeeded = true,
            StatusCode = (int)statusCode,
            Message = message,
            Data = data
        };
    }

    public static Response<object?> Error(string message = "Internal Server Error", object? errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new Response<object?>
        {
            Succeeded = false,
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors is string str ? new[] { str } : errors as string[] ?? new[] { "Unknown error" }
        };
    }
}
