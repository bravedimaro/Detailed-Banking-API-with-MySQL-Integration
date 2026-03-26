using BankingAPI.Application.Features.Auth.Commands;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace BankingAPI.Application.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IJwtService> _jwt = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_users.Object, _refreshTokens.Object, _jwt.Object, _uow.Object);
        _jwt.Setup(j => j.AccessTokenExpiresAt).Returns(DateTime.UtcNow.AddMinutes(15));
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "john@example.com",
            FullName = "John Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1")
        };

        _users.Setup(r => r.GetByEmailAsync("john@example.com", default)).ReturnsAsync(user);
        _jwt.Setup(j => j.GenerateAccessToken(user)).Returns("access-token");
        _jwt.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _handler.Handle(new LoginCommand("john@example.com", "Password1"), default);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        _refreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), default), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new LoginCommand("nobody@example.com", "Password1"), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsDomainException()
    {
        var user = new User
        {
            Email = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1")
        };

        _users.Setup(r => r.GetByEmailAsync("john@example.com", default)).ReturnsAsync(user);

        var act = () => _handler.Handle(new LoginCommand("john@example.com", "WrongPassword1"), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid credentials.");
    }
}
