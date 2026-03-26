using BankingAPI.Application.DTOs;
using BankingAPI.Application.Interfaces;
using BankingAPI.Domain.Exceptions;
using MediatR;

namespace BankingAPI.Application.Features.Users.Commands;

public record UpdateProfileCommand(Guid UserId, string? FullName, string? PhoneNumber, string? Address) : IRequest<UserProfileResponse>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileResponse>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UpdateProfileCommandHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<UserProfileResponse> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

        if (!string.IsNullOrWhiteSpace(request.FullName)) user.FullName = request.FullName;
        if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
        if (request.Address is not null) user.Address = request.Address;

        await _users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new UserProfileResponse(user.Id, user.FullName, user.Email, user.PhoneNumber, user.Address, user.CreatedAt);
    }
}
