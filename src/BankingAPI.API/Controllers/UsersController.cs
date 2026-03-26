using BankingAPI.API.Extensions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Users.Commands;
using BankingAPI.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankingAPI.API.Controllers;

/// <summary>
/// Authenticated user profile management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Authenticated user profile management")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("me")]
    [SwaggerOperation(
        Summary = "Get current user profile",
        Description = "Returns the full profile of the currently authenticated user, including contact details and registration date.")]
    [SwaggerResponse(200, "Profile retrieved successfully", typeof(UserProfileResponse))]
    [SwaggerResponse(401, "Authentication required")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileQuery(User.GetUserId()), ct);
        return Ok(result);
    }

    [HttpPut("me")]
    [SwaggerOperation(
        Summary = "Update current user profile",
        Description = @"Updates the contact information of the currently authenticated user.

**Updatable fields:**
- `fullName` — Display name (max 100 characters)
- `phoneNumber` — Contact phone number
- `address` — Mailing address

**Note:** Email address cannot be changed after registration.")]
    [SwaggerResponse(200, "Profile updated successfully", typeof(UserProfileResponse))]
    [SwaggerResponse(400, "Validation failed")]
    [SwaggerResponse(401, "Authentication required")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateProfileCommand(User.GetUserId(), request.FullName, request.PhoneNumber, request.Address), ct);
        return Ok(result);
    }
}
