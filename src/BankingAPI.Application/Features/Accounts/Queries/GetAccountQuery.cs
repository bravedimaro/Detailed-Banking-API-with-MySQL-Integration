using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Accounts.Queries;

public record GetAccountQuery(Guid UserId) : IRequest<AccountResponse>;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountResponse>
{
    private readonly IAccountRepository _accounts;
    public GetAccountQueryHandler(IAccountRepository accounts) => _accounts = accounts;

    public async Task<AccountResponse> Handle(GetAccountQuery request, CancellationToken ct)
    {
        var account = await _accounts.GetByUserIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.UserId);

        return new AccountResponse(account.AccountNumber, account.Balance, account.CreatedAt, account.UpdatedAt);
    }
}
