using System.Collections.ObjectModel;
using FinanceBuddy.Models;

namespace FinanceBuddy.Services;

/// <summary>
/// Заглушка слоя Data. Используется как простой источник данных в памяти.
/// На этапе PR 09-10 приложение переключено на EF Core (SQLite), но заглушка оставлена для демонстрации.
/// </summary>
public sealed class InMemoryFinanceRepository : IFinanceRepository
{
    public ObservableCollection<string> Categories { get; } = new()
    {
        "Продукты",
        "Транспорт",
        "Дом",
        "Развлечения",
        "Здоровье",
        "Зарплата",
        "Другое",
    };

    public ObservableCollection<Transaction> Transactions { get; } = new();

    public InMemoryFinanceRepository()
    {
        // тестовые данные
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

    public void Reload()
    {
        // in-memory: ничего не делаем
    }

    public void AddCategory(string name)
    {
        name = Normalize(name);
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (!Categories.Contains(name))
            Categories.Add(name);
    }

    public void DeleteCategory(string name)
    {
        name = Normalize(name);
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (Transactions.Any(t => string.Equals(t.Category, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Нельзя удалить категорию, которая используется в операциях.");

        Categories.Remove(name);
    }

    public void AddTransaction(Transaction tx)
    {
        Transactions.Add(tx);
    }

    public void UpdateTransaction(Transaction tx)
    {
        var existing = Transactions.FirstOrDefault(x => x.Id == tx.Id);
        if (existing is null)
            return;

        existing.Date = tx.Date;
        existing.Type = tx.Type;
        existing.Category = tx.Category;
        existing.Description = tx.Description;
        existing.Amount = tx.Amount;
    }

    public void DeleteTransaction(Guid id)
    {
        var tx = Transactions.FirstOrDefault(x => x.Id == id);
        if (tx != null)
            Transactions.Remove(tx);
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();
}
