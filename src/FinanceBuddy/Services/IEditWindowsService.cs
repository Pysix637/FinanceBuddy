using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

public interface IEditWindowsService
{
    bool EditTransaction(Transaction tx, IReadOnlyList<string> categories);
    void ManageCategories();
}
