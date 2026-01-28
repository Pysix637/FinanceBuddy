namespace FinanceBuddy.Models;

public readonly record struct FinanceSummary(decimal TotalIncome, decimal TotalExpense)
{
    public decimal Balance => TotalIncome - TotalExpense;
}
