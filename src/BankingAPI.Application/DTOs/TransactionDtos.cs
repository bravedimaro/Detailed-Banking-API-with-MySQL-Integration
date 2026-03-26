namespace BankingAPI.Application.DTOs;

public record TransferRequest(string ReceiverAccountNumber, decimal Amount, string? Description);

public record TransactionResponse(
    Guid Id,
    string SenderAccountNumber,
    string ReceiverAccountNumber,
    decimal Amount,
    string? Description,
    string Status,
    DateTime CreatedAt
);
