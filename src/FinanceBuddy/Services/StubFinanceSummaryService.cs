using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

// Заглушка сервиса расчета итогов.
// На следующем этапе можно заменить на расчет из базы данных или отдельный модуль аналитики.
public sealed class StubFinanceSummaryService : IFinanceSummaryService
{
    public FinanceSummary Calculate(IEnumerable<Transaction> transactions)
    {
        var totalIncome = 0m;
        var totalExpense = 0m;

        foreach (var t in transactions)
        {
            if (t.Type == TransactionType.Income) totalIncome += t.Amount;
            if (t.Type == TransactionType.Expense) totalExpense += t.Amount;
        }

        return new FinanceSummary(totalIncome, totalExpense);
    }
}
