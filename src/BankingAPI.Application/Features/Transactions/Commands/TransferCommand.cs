using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Transactions.Commands;

public record TransferCommand(Guid SenderUserId, string ReceiverAccountNumber, decimal Amount, string? Description)
    : IRequest<TransactionResponse>;

public class TransferCommandHandler : IRequestHandler<TransferCommand, TransactionResponse>
{
    private readonly IAccountRepository _accounts;
    private readonly ITransactionRepository _transactions;
    private readonly IUnitOfWork _uow;

    public TransferCommandHandler(
        IAccountRepository accounts,
        ITransactionRepository transactions,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _transactions = transactions;
        _uow = uow;
    }

    public async Task<TransactionResponse> Handle(TransferCommand request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            throw new DomainException("Transfer amount must be greater than zero.");

        var senderAccount = await _accounts.GetByUserIdAsync(request.SenderUserId, ct)
            ?? throw new NotFoundException(nameof(Account), request.SenderUserId);

        var receiverAccount = await _accounts.GetByAccountNumberAsync(request.ReceiverAccountNumber, ct)
            ?? throw new NotFoundException(nameof(Account), request.ReceiverAccountNumber);

        if (senderAccount.Id == receiverAccount.Id)
            throw new DomainException("Cannot transfer to the same account.");

        if (senderAccount.Balance < request.Amount)
            throw new InsufficientFundsException();

        senderAccount.Balance -= request.Amount;
        senderAccount.UpdatedAt = DateTime.UtcNow;

        receiverAccount.Balance += request.Amount;
        receiverAccount.UpdatedAt = DateTime.UtcNow;

        var transaction = new Transaction
        {
            SenderAccountId = senderAccount.Id,
            ReceiverAccountId = receiverAccount.Id,
            Amount = request.Amount,
            Description = request.Description,
            Status = TransactionStatus.Completed
        };

        await _accounts.UpdateAsync(senderAccount, ct);
        await _accounts.UpdateAsync(receiverAccount, ct);
        await _transactions.AddAsync(transaction, ct);
        await _uow.SaveChangesAsync(ct);

        return new TransactionResponse(
            transaction.Id,
            senderAccount.AccountNumber,
            receiverAccount.AccountNumber,
            transaction.Amount,
            transaction.Description,
            transaction.Status.ToString(),
            transaction.CreatedAt
        );
    }
}
