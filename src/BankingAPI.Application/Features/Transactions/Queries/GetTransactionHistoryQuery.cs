using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Transactions.Queries;

public record GetTransactionHistoryQuery(Guid UserId) : IRequest<IEnumerable<TransactionResponse>>;

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<TransactionResponse>>
{
    private readonly IAccountRepository _accounts;
    private readonly ITransactionRepository _transactions;

    public GetTransactionHistoryQueryHandler(IAccountRepository accounts, ITransactionRepository transactions)
    {
        _accounts = accounts;
        _transactions = transactions;
    }

    public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionHistoryQuery request, CancellationToken ct)
    {
        var account = await _accounts.GetByUserIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.UserId);

        var txns = await _transactions.GetByAccountIdAsync(account.Id, ct);

        return txns.Select(t => new TransactionResponse(
            t.Id,
            t.SenderAccount?.AccountNumber ?? string.Empty,
            t.ReceiverAccount?.AccountNumber ?? string.Empty,
            t.Amount,
            t.Description,
            t.Status.ToString(),
            t.CreatedAt
        ));
    }
}
