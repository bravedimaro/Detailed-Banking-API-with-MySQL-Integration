using BankingAPI.Application.Features.Transactions.Commands;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace BankingAPI.Application.Tests.Features.Transactions;

public class TransferCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accounts = new();
    private readonly Mock<ITransactionRepository> _transactions = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly TransferCommandHandler _handler;

    public TransferCommandHandlerTests()
    {
        _handler = new TransferCommandHandler(_accounts.Object, _transactions.Object, _uow.Object);
    }

    private static Account MakeAccount(decimal balance, string? number = null) => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        AccountNumber = number ?? $"ACC{Guid.NewGuid():N}",
        Balance = balance
    };

    [Fact]
    public async Task Handle_ValidTransfer_UpdatesBalancesAndLogsTransaction()
    {
        var sender = MakeAccount(500m, "ACC001");
        var receiver = MakeAccount(100m, "ACC002");

        _accounts.Setup(r => r.GetByUserIdAsync(sender.UserId, default)).ReturnsAsync(sender);
        _accounts.Setup(r => r.GetByAccountNumberAsync("ACC002", default)).ReturnsAsync(receiver);

        var command = new TransferCommand(sender.UserId, "ACC002", 200m, "Test transfer");
        var result = await _handler.Handle(command, default);

        sender.Balance.Should().Be(300m);
        receiver.Balance.Should().Be(300m);
        result.Amount.Should().Be(200m);
        result.Status.Should().Be("Completed");
        _transactions.Verify(r => r.AddAsync(It.IsAny<Transaction>(), default), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientFunds_ThrowsInsufficientFundsException()
    {
        var sender = MakeAccount(50m, "ACC001");
        var receiver = MakeAccount(100m, "ACC002");

        _accounts.Setup(r => r.GetByUserIdAsync(sender.UserId, default)).ReturnsAsync(sender);
        _accounts.Setup(r => r.GetByAccountNumberAsync("ACC002", default)).ReturnsAsync(receiver);

        var act = () => _handler.Handle(new TransferCommand(sender.UserId, "ACC002", 200m, null), default);

        await act.Should().ThrowAsync<InsufficientFundsException>();
    }

    [Fact]
    public async Task Handle_ZeroAmount_ThrowsDomainException()
    {
        var sender = MakeAccount(500m, "ACC001");
        var receiver = MakeAccount(100m, "ACC002");

        _accounts.Setup(r => r.GetByUserIdAsync(sender.UserId, default)).ReturnsAsync(sender);
        _accounts.Setup(r => r.GetByAccountNumberAsync("ACC002", default)).ReturnsAsync(receiver);

        var act = () => _handler.Handle(new TransferCommand(sender.UserId, "ACC002", 0m, null), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_SameAccount_ThrowsDomainException()
    {
        var account = MakeAccount(500m, "ACC001");
        account.UserId = Guid.NewGuid();

        _accounts.Setup(r => r.GetByUserIdAsync(account.UserId, default)).ReturnsAsync(account);
        _accounts.Setup(r => r.GetByAccountNumberAsync("ACC001", default)).ReturnsAsync(account);

        var act = () => _handler.Handle(new TransferCommand(account.UserId, "ACC001", 100m, null), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Cannot transfer to the same account.");
    }
}
