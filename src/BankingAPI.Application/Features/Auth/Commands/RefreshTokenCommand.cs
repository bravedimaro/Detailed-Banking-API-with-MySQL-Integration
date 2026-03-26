using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        IJwtService jwt,
        IUnitOfWork uow)
    {
        _refreshTokens = refreshTokens;
        _users = users;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var stored = await _refreshTokens.GetActiveTokenAsync(request.RefreshToken, ct)
            ?? throw new DomainException("Invalid or expired refresh token.");

        var user = await _users.GetByIdAsync(stored.UserId, ct)
            ?? throw new NotFoundException(nameof(User), stored.UserId);

        // Rotate: revoke old, issue new
        stored.IsRevoked = true;

        var newRawRefresh = _jwt.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newRawRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _refreshTokens.AddAsync(newRefreshToken, ct);
        await _uow.SaveChangesAsync(ct);

        return new LoginResponse(_jwt.GenerateAccessToken(user), newRawRefresh, _jwt.AccessTokenExpiresAt);
    }
}
