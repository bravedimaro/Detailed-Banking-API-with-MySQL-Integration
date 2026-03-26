using BankingAPI.API.Extensions;
using BankingAPI.Application.DTOs;
using BankingAPI.Application.Features.Transactions.Commands;
using BankingAPI.Application.Features.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.API.Controllers;

/// <summary>Fund transfer and transaction history endpoints</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TransactionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Transfer funds to another account</summary>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new TransferCommand(User.GetUserId(), request.ReceiverAccountNumber, request.Amount, request.Description), ct);
        return CreatedAtAction(nameof(Transfer), result);
    }

    /// <summary>Get the authenticated user's transaction history</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTransactionHistoryQuery(User.GetUserId()), ct);
        return Ok(result);
    }
}
