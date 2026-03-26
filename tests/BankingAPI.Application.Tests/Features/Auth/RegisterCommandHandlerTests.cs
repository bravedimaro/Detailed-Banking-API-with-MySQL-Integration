using BankingAPI.Application.Features.Auth.Commands;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace BankingAPI.Application.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IAccountRepository> _accounts = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_users.Object, _accounts.Object, _uow.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndAccount()
    {
        _users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(false);

        await _handler.Handle(new RegisterCommand("John Doe", "john@example.com", "Password1"), default);

        _users.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
        _accounts.Verify(r => r.AddAsync(It.IsAny<Account>(), default), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        _users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new RegisterCommand("Jane Doe", "jane@example.com", "Password1"), default);

        await act.Should().ThrowAsync<DuplicateEmailException>();
    }
}
