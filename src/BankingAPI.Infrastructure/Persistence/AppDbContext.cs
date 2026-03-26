using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => a.AccountNumber).IsUnique();
            e.Property(a => a.Balance).HasPrecision(18, 2);
            e.HasOne(a => a.User)
             .WithOne(u => u.Account)
             .HasForeignKey<Account>(a => a.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasOne(t => t.SenderAccount)
             .WithMany(a => a.SentTransactions)
             .HasForeignKey(t => t.SenderAccountId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ReceiverAccount)
             .WithMany(a => a.ReceivedTransactions)
             .HasForeignKey(t => t.ReceiverAccountId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}
