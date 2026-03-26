using BankingAPI.API.Extensions;
using BankingAPI.Application.Common;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Transactions.Commands;
using BankingAPI.Application.Features.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankingAPI.API.Controllers;

/// <summary>
/// Fund transfers and transaction history
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Fund transfers and transaction history")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TransactionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("transfer")]
    [SwaggerOperation(
        Summary = "Initiate a fund transfer",
        Description = @"Transfers funds from the authenticated user's account to a destination account.

**Validation Rules:**
- `receiverAccountNumber` — Required, must match an existing account
- `amount` — Required, must be greater than zero
- `description` — Optional, max 200 characters

**Business Rules:**
- Sender must have sufficient balance
- Transfers to the same account are not permitted
- Both balances are updated atomically")]
    [SwaggerResponse(201, "Transfer completed successfully", typeof(ApiResponse<TransactionResponse>))]
    [SwaggerResponse(400, "Validation failed or business rule violation", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Sender or receiver account not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(422, "Insufficient funds", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new TransferCommand(User.GetUserId(), request.ReceiverAccountNumber, request.Amount, request.Description), ct);
        return StatusCode(201, ApiResponse<TransactionResponse>.Success(result, "Transfer completed successfully."));
    }

    [HttpGet("history")]
    [SwaggerOperation(
        Summary = "Get transaction history",
        Description = @"Returns all transactions where the authenticated user's account is either the sender or receiver, ordered by date descending.")]
    [SwaggerResponse(200, "Transaction history retrieved", typeof(ApiResponse<IEnumerable<TransactionResponse>>))]
    [SwaggerResponse(401, "Authentication required", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Account not found", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTransactionHistoryQuery(User.GetUserId()), ct);
        return Ok(ApiResponse<IEnumerable<TransactionResponse>>.Success(result, "Transaction history retrieved successfully."));
    }
}
