using FinanceBuddy.Data.Entities;
using FinanceBuddy.Models;

namespace FinanceBuddy.Data;

public static class DbSeeder
{
    public static void Seed(FinanceBuddyDbContext db)
    {
        // Категории
        if (!db.Categories.Any())
        {
            var categories = new[]
            {
                new CategoryEntity { Name = "Продукты" },
                new CategoryEntity { Name = "Транспорт" },
                new CategoryEntity { Name = "Дом" },
                new CategoryEntity { Name = "Развлечения" },
                new CategoryEntity { Name = "Здоровье" },
                new CategoryEntity { Name = "Зарплата" },
                new CategoryEntity { Name = "Другое" },
            };

            db.Categories.AddRange(categories);
            db.SaveChanges();
        }

        // Тестовые операции
        if (!db.Transactions.Any())
        {
            db.Transactions.AddRange(
                new TransactionEntity
                {
                    Date = DateTime.Today.AddDays(-2),
                    Type = TransactionType.Expense,
                    CategoryName = "Продукты",
                    Description = "Супермаркет",
                    Amount = 23.40m
                },
                new TransactionEntity
                {
                    Date = DateTime.Today.AddDays(-1),
                    Type = TransactionType.Income,
                    CategoryName = "Зарплата",
                    Description = "Аванс",
                    Amount = 350.00m
                },
                new TransactionEntity
                {
                    Date = DateTime.Today,
                    Type = TransactionType.Expense,
                    CategoryName = "Транспорт",
                    Description = "Проезд",
                    Amount = 2.50m
                }
            );

            db.SaveChanges();
        }
    }
}
