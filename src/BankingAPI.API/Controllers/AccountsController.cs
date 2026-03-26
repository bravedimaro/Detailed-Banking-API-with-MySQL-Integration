using BankingAPI.API.Extensions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankingAPI.API.Controllers;

/// <summary>
/// Bank account details and balance enquiry
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Bank account details and balance enquiry")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("me")]
    [SwaggerOperation(
        Summary = "Get account details",
        Description = @"Returns the bank account linked to the currently authenticated user.

**Response includes:**
- `accountNumber` — The unique account number assigned at registration
- `balance` — Current available balance (2 decimal places)
- `createdAt` / `updatedAt` — Account creation and last-modified timestamps")]
    [SwaggerResponse(200, "Account details retrieved", typeof(AccountResponse))]
    [SwaggerResponse(401, "Authentication required")]
    [SwaggerResponse(404, "Account not found for this user")]
    public async Task<IActionResult> GetMyAccount(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetAccountQuery(userId), ct);
        return Ok(result);
    }
}
