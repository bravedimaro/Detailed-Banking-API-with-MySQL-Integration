using BankingAPI.API.Extensions;
using BankingAPI.Application.Common;
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
        Description = "Returns the profile of the currently authenticated user. User ID is not exposed.")]
    [SwaggerResponse(200, "Profile retrieved successfully", typeof(ApiResponse<UserProfileResponse>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileQuery(User.GetUserId()), ct);
        return Ok(ApiResponse<UserProfileResponse>.Success(result, "Profile retrieved successfully."));
    }

    [HttpPut("me")]
    [SwaggerOperation(
        Summary = "Update current user profile",
        Description = @"Updates the contact information of the currently authenticated user.

**Updatable fields:** `fullName`, `phoneNumber`, `address`

**Note:** Email address cannot be changed after registration.")]
    [SwaggerResponse(200, "Profile updated successfully", typeof(ApiResponse<object>))]
    [SwaggerResponse(400, "Validation failed", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        await _mediator.Send(
            new UpdateProfileCommand(User.GetUserId(), request.FullName, request.PhoneNumber, request.Address), ct);
        return Ok(ApiResponse<object>.Success("Profile updated successfully."));
    }
}
