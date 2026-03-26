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
    private readonly Mock<IJwtService> _jwt = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_users.Object, _accounts.Object, _jwt.Object, _uow.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsAuthResponse()
    {
        // Arrange
        _users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(false);
        _jwt.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        var command = new RegisterCommand("John Doe", "john@example.com", "Password1");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Token.Should().Be("test-token");
        result.Email.Should().Be("john@example.com");
        _users.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
        _accounts.Verify(r => r.AddAsync(It.IsAny<Account>(), default), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        // Arrange
        _users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(true);

        var command = new RegisterCommand("Jane Doe", "jane@example.com", "Password1");

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }
}
