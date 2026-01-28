using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

public sealed class FinanceSummaryService : IFinanceSummaryService
{
    public FinanceSummary Calculate(IEnumerable<Transaction> transactions)
    {
        var income = 0m;
        var expense = 0m;

        foreach (var t in transactions)
        {
            if (t.Type == TransactionType.Income)
                income += t.Amount;
            else
                expense += t.Amount;
        }

        return new FinanceSummary(income, expense);
    }
}
