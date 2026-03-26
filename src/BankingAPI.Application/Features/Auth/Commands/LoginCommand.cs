using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IUserRepository users, IJwtService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email.ToLowerInvariant(), ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.Email);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid credentials.");

        return new AuthResponse(_jwt.GenerateToken(user), user.Email, user.FullName, user.Id);
    }
}
