using BankingAPI.API.Extensions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Users.Commands;
using BankingAPI.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.API.Controllers;

/// <summary>User profile endpoints</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get the authenticated user's profile</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileQuery(User.GetUserId()), ct);
        return Ok(result);
    }

    /// <summary>Update the authenticated user's profile</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateProfileCommand(User.GetUserId(), request.FullName, request.PhoneNumber, request.Address), ct);
        return Ok(result);
    }
}
