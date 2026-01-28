using System.Collections.ObjectModel;
using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

public interface IFinanceRepository
{
    ObservableCollection<string> Categories { get; }
    ObservableCollection<Transaction> Transactions { get; }

    void Reload();

    void AddCategory(string name);
    void DeleteCategory(string name);

    void AddTransaction(Transaction tx);
    void UpdateTransaction(Transaction tx);
    void DeleteTransaction(Guid id);
}
