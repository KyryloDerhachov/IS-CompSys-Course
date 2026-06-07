using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Course.WebApi.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductClass> ProductClasses => Set<ProductClass>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Supply> Supplies => Set<Supply>();
    public DbSet<SupplyItem> SupplyItems => Set<SupplyItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<ReturnTransaction> ReturnTransactions => Set<ReturnTransaction>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();
    public DbSet<FeedbackRecord> FeedbackRecords => Set<FeedbackRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}