using BankingAPI.API.Extensions;
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
- Sender must have sufficient balance to cover the transfer amount
- Transfers to the same account are not permitted
- Both sender and receiver balances are updated atomically

**On success:** A transaction record is created with status `Completed`.")]
    [SwaggerResponse(201, "Transfer completed successfully", typeof(TransactionResponse))]
    [SwaggerResponse(400, "Validation failed or business rule violation")]
    [SwaggerResponse(401, "Authentication required")]
    [SwaggerResponse(404, "Sender or receiver account not found")]
    [SwaggerResponse(422, "Insufficient funds")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new TransferCommand(User.GetUserId(), request.ReceiverAccountNumber, request.Amount, request.Description), ct);
        return CreatedAtAction(nameof(Transfer), result);
    }

    [HttpGet("history")]
    [SwaggerOperation(
        Summary = "Get transaction history",
        Description = @"Returns all transactions where the authenticated user's account is either the sender or receiver.

**Response ordering:** Transactions are returned in descending order by date (most recent first).

**Each record includes:**
- `senderAccountNumber` / `receiverAccountNumber`
- `amount` and optional `description`
- `status` — `Completed`, `Pending`, or `Failed`
- `createdAt` — UTC timestamp of the transaction")]
    [SwaggerResponse(200, "Transaction history retrieved", typeof(IEnumerable<TransactionResponse>))]
    [SwaggerResponse(401, "Authentication required")]
    [SwaggerResponse(404, "Account not found for this user")]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTransactionHistoryQuery(User.GetUserId()), ct);
        return Ok(result);
    }
}
