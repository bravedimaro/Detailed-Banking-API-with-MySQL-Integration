using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Auth.Commands;

public record LogoutCommand(Guid UserId) : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokens, IUnitOfWork uow)
    {
        _refreshTokens = refreshTokens;
        _uow = uow;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        await _refreshTokens.RevokeAllForUserAsync(request.UserId, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
