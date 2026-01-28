using FinanceBuddy.Models;

namespace FinanceBuddy.Data.Entities;

public sealed class TransactionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Today;
    public TransactionType Type { get; set; } = TransactionType.Expense;

    public string CategoryName { get; set; } = "Другое";
    public CategoryEntity? Category { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
