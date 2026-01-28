using System.Collections.ObjectModel;
using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

public sealed class InMemoryFinanceRepository
{
    public ObservableCollection<string> Categories { get; } = new()
    {
        "Продукты",
        "Транспорт",
        "Дом",
        "Развлечения",
        "Здоровье",
        "Зарплата",
        "Другое"
    };

    public ObservableCollection<Transaction> Transactions { get; } = new();

    public InMemoryFinanceRepository()
    {
        // демо‑данные для проверки UI
        Transactions.Add(new Transaction
        {
            Date = DateTime.Today.AddDays(-2),
            Type = TransactionType.Expense,
            Category = "Продукты",
            Description = "Супермаркет",
            Amount = 23.40m
        });

        Transactions.Add(new Transaction
        {
            Date = DateTime.Today.AddDays(-1),
            Type = TransactionType.Income,
            Category = "Зарплата",
            Description = "Аванс",
            Amount = 350.00m
        });
    }

    public void AddTransaction(Transaction tx) => Transactions.Add(tx);

    public void DeleteTransaction(Guid id)
    {
        var target = Transactions.FirstOrDefault(t => t.Id == id);
        if (target != null) Transactions.Remove(target);
    }
}
