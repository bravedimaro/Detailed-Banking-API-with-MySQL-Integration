using BankingAPI.API.Extensions;
using BankingAPI.Application.Common;
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
        Description = "Returns the bank account linked to the currently authenticated user. Account ID is not exposed.")]
    [SwaggerResponse(200, "Account details retrieved", typeof(ApiResponse<AccountResponse>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Account not found", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetMyAccount(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAccountQuery(User.GetUserId()), ct);
        return Ok(ApiResponse<AccountResponse>.Success(result, "Account details retrieved successfully."));
    }
}
