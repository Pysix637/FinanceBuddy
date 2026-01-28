using System.Collections.ObjectModel;
using FinanceBuddy.Data;
using FinanceBuddy.Data.Entities;
using FinanceBuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceBuddy.Services;

/// <summary>
/// Реальный слой Data на EF Core (SQLite).
/// Хранит данные в локальной базе и синхронизирует их с ObservableCollection для UI.
/// </summary>
public sealed class EfFinanceRepository : IFinanceRepository
{
    private readonly IDbContextFactory<FinanceBuddyDbContext> _dbFactory;

    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<Transaction> Transactions { get; } = new();

    public EfFinanceRepository(IDbContextFactory<FinanceBuddyDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        Reload();
    }

    public void Reload()
    {
        using var db = _dbFactory.CreateDbContext();

        // категории
        Categories.Clear();
        foreach (var c in db.Categories.AsNoTracking().OrderBy(x => x.Name))
            Categories.Add(c.Name);

        if (!Categories.Contains("Другое"))
            Categories.Add("Другое");

        // операции
        Transactions.Clear();
        foreach (var t in db.Transactions.AsNoTracking().OrderByDescending(x => x.Date))
        {
            Transactions.Add(new Transaction
            {
                Id = t.Id,
                Date = t.Date,
                Type = t.Type,
                Category = t.CategoryName,
                Description = t.Description,
                Amount = t.Amount
            });
        }
    }

    public void AddCategory(string name)
    {
        name = Normalize(name);
        if (string.IsNullOrWhiteSpace(name))
            return;

        using var db = _dbFactory.CreateDbContext();
        var exists = db.Categories.Any(x => x.Name == name);
        if (!exists)
        {
            db.Categories.Add(new CategoryEntity { Name = name });
            db.SaveChanges();
        }

        if (!Categories.Contains(name))
            Categories.Add(name);
    }

    public void DeleteCategory(string name)
    {
        name = Normalize(name);
        if (string.IsNullOrWhiteSpace(name))
            return;

        using var db = _dbFactory.CreateDbContext();

        var used = db.Transactions.Any(x => x.CategoryName == name);
        if (used)
            throw new InvalidOperationException("Нельзя удалить категорию, которая используется в операциях.");

        var entity = db.Categories.FirstOrDefault(x => x.Name == name);
        if (entity is null)
            return;

        db.Categories.Remove(entity);
        db.SaveChanges();

        Categories.Remove(name);
    }

    public void AddTransaction(Transaction tx)
    {
        using var db = _dbFactory.CreateDbContext();

        EnsureCategory(db, tx.Category);

        var entity = new TransactionEntity
        {
            Id = tx.Id,
            Date = tx.Date,
            Type = tx.Type,
            CategoryName = Normalize(tx.Category),
            Description = tx.Description ?? string.Empty,
            Amount = tx.Amount
        };

        db.Transactions.Add(entity);
        db.SaveChanges();

        Transactions.Insert(0, tx);
    }

    public void UpdateTransaction(Transaction tx)
    {
        using var db = _dbFactory.CreateDbContext();

        EnsureCategory(db, tx.Category);

        var entity = db.Transactions.FirstOrDefault(x => x.Id == tx.Id);
        if (entity is null)
            return;

        entity.Date = tx.Date;
        entity.Type = tx.Type;
        entity.CategoryName = Normalize(tx.Category);
        entity.Description = tx.Description ?? string.Empty;
        entity.Amount = tx.Amount;

        db.SaveChanges();

        // UI-объект tx обычно тот же, но на всякий случай обновим коллекцию
        var existing = Transactions.FirstOrDefault(x => x.Id == tx.Id);
        if (existing is not null && !ReferenceEquals(existing, tx))
        {
            existing.Date = tx.Date;
            existing.Type = tx.Type;
            existing.Category = tx.Category;
            existing.Description = tx.Description;
            existing.Amount = tx.Amount;
        }
    }

    public void DeleteTransaction(Guid id)
    {
        using var db = _dbFactory.CreateDbContext();

        var entity = db.Transactions.FirstOrDefault(x => x.Id == id);
        if (entity is null)
            return;

        db.Transactions.Remove(entity);
        db.SaveChanges();

        var tx = Transactions.FirstOrDefault(x => x.Id == id);
        if (tx != null)
            Transactions.Remove(tx);
    }

    private void EnsureCategory(FinanceBuddyDbContext db, string categoryName)
    {
        categoryName = Normalize(categoryName);
        if (string.IsNullOrWhiteSpace(categoryName))
            categoryName = "Другое";

        var exists = db.Categories.Any(x => x.Name == categoryName);
        if (!exists)
        {
            db.Categories.Add(new CategoryEntity { Name = categoryName });
            db.SaveChanges();

            if (!Categories.Contains(categoryName))
                Categories.Add(categoryName);
        }
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();
}
