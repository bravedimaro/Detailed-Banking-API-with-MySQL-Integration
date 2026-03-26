using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Auth.Commands;
using MediatR;
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
        Description = @"Creates a new user account and returns a JWT token.

**Validation Rules:**
- `fullName` — Required, max 100 characters
- `email` — Required, must be a valid email address
- `password` — Required, min 8 characters, must contain at least one uppercase letter and one digit

**On success:** A bank account is automatically created and linked to the user.")]
    [SwaggerResponse(201, "User registered successfully", typeof(AuthResponse))]
    [SwaggerResponse(400, "Validation failed")]
    [SwaggerResponse(409, "Email address already in use")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterCommand(request.FullName, request.Email, request.Password), ct);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Authenticate a user",
        Description = @"Validates credentials and returns a signed JWT token.

**Token usage:** Include the token in subsequent requests as:
```
Authorization: Bearer <token>
```

**Token expiry:** Tokens are valid for 24 hours.")]
    [SwaggerResponse(200, "Login successful — JWT token returned", typeof(AuthResponse))]
    [SwaggerResponse(400, "Invalid credentials")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }
}
