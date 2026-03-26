using BankingAPI.API.Extensions;
using BankingAPI.Application.Common;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankingAPI.API.Controllers;

/// <summary>
/// User registration and authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("User registration and authentication operations")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = @"Creates a new user account. Login separately to obtain tokens.

**Validation Rules:**
- `fullName` — Required, max 100 characters
- `email` — Required, valid email address
- `password` — Required, min 8 characters, at least one uppercase letter and one digit

**On success:** A bank account is automatically created and linked to the user.")]
    [SwaggerResponse(201, "User registered successfully", typeof(ApiResponse<object>))]
    [SwaggerResponse(400, "Validation failed", typeof(ApiResponse<object>))]
    [SwaggerResponse(409, "Email address already in use", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        await _mediator.Send(new RegisterCommand(request.FullName, request.Email, request.Password), ct);
        return StatusCode(201, ApiResponse<object>.Success("Registration successful. Please login to continue."));
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Authenticate a user",
        Description = @"Validates credentials and returns an access token and a refresh token.

**Access token:** Short-lived (15 min by default). Pass as `Authorization: Bearer <token>`.

**Refresh token:** Long-lived (30 days). Use `POST /api/auth/refresh` to rotate.")]
    [SwaggerResponse(200, "Login successful", typeof(ApiResponse<LoginResponse>))]
    [SwaggerResponse(400, "Invalid credentials", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);
        return Ok(ApiResponse<LoginResponse>.Success(result, "Login successful."));
    }

    [HttpPost("refresh")]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = @"Exchanges a valid refresh token for a new access token and a rotated refresh token.

**Note:** The old refresh token is immediately revoked on use.")]
    [SwaggerResponse(200, "Tokens refreshed", typeof(ApiResponse<LoginResponse>))]
    [SwaggerResponse(400, "Invalid or expired refresh token", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);
        return Ok(ApiResponse<LoginResponse>.Success(result, "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Logout",
        Description = "Revokes all active refresh tokens for the authenticated user. The access token remains valid until it naturally expires.")]
    [SwaggerResponse(200, "Logged out successfully", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await _mediator.Send(new LogoutCommand(User.GetUserId()), ct);
        return Ok(ApiResponse<object>.Success("Logged out successfully."));
    }
}
