namespace FinanceBuddy.Models;

public enum TransactionType
{
    Income,
    Expense
}

public sealed class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Today;
    public TransactionType Type { get; set; } = TransactionType.Expense;
    public string Category { get; set; } = "Другое";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}
