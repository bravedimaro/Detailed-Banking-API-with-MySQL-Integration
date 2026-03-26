using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;
    public AccountRepository(AppDbContext db) => _db = db;

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, ct);

    public Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default)
        => _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, ct);

    public async Task AddAsync(Account account, CancellationToken ct = default)
        => await _db.Accounts.AddAsync(account, ct);

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        _db.Accounts.Update(account);
        return Task.CompletedTask;
    }
}
