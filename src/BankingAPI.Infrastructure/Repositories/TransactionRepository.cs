using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;
    public TransactionRepository(AppDbContext db) => _db = db;

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Transactions
              .Include(t => t.SenderAccount)
              .Include(t => t.ReceiverAccount)
              .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
        => await _db.Transactions
                    .Include(t => t.SenderAccount)
                    .Include(t => t.ReceiverAccount)
                    .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        => await _db.Transactions.AddAsync(transaction, ct);
}
