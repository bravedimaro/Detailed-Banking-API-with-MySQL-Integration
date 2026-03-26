using BankingAPI.Application.Features.Transactions.Commands;
using FluentValidation;

namespace BankingAPI.Application.Validators;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(x => x.ReceiverAccountNumber).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
