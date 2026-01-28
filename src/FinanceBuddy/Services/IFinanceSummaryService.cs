using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

public interface IFinanceSummaryService
{
    FinanceSummary Calculate(IEnumerable<Transaction> transactions);
}
