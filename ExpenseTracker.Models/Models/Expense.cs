namespace ExpenseTracker.Models.Models;

public class Expense
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ExpenseCategory? Category { get; set; }
}
