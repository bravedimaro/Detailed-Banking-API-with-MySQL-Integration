using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResponse>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IAccountRepository _accounts;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(
        IUserRepository users,
        IAccountRepository accounts,
        IJwtService jwt,
        IUnitOfWork uow)
    {
        _users = users;
        _accounts = accounts;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(request.Email, ct))
            throw new DuplicateEmailException(request.Email);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.AddAsync(user, ct);

        var account = new Account
        {
            UserId = user.Id,
            AccountNumber = GenerateAccountNumber()
        };

        await _accounts.AddAsync(account, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse(_jwt.GenerateToken(user), user.Email, user.FullName, user.Id);
    }

    private static string GenerateAccountNumber()
        => $"ACC{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Random.Shared.Next(100, 999)}";
}
