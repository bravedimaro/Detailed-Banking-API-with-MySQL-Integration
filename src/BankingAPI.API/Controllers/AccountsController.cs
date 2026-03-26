using BankingAPI.API.Extensions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.API.Controllers;

/// <summary>Account management endpoints</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get the authenticated user's account details and balance</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyAccount(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetAccountQuery(userId), ct);
        return Ok(result);
    }
}
