using FinanceBuddy.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceBuddy.Data;

public sealed class FinanceBuddyDbContext : DbContext
{
    public FinanceBuddyDbContext(DbContextOptions<FinanceBuddyDbContext> options) : base(options)
    {
    }

    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>(b =>
        {
            b.ToTable("Categories");
            b.HasKey(x => x.Name);
            b.Property(x => x.Name).IsRequired().HasMaxLength(64);
        });

        modelBuilder.Entity<TransactionEntity>(b =>
        {
            b.ToTable("Transactions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Date).IsRequired();
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.Description).HasMaxLength(256);
            b.Property(x => x.Amount).HasPrecision(18, 2);

            b.Property(x => x.CategoryName).IsRequired().HasMaxLength(64);

            b.HasOne(x => x.Category)
             .WithMany()
             .HasForeignKey(x => x.CategoryName)
             .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
