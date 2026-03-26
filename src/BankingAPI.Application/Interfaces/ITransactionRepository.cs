using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
}
