using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    Task UpdateAsync(Account account, CancellationToken ct = default);
}
