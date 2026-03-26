using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Users.Queries;

public record GetProfileQuery(Guid UserId) : IRequest<UserProfileResponse>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private readonly IUserRepository _users;

    public GetProfileQueryHandler(IUserRepository users) => _users = users;

    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

        return new UserProfileResponse(user.Id, user.FullName, user.Email, user.PhoneNumber, user.Address, user.CreatedAt);
    }
}
