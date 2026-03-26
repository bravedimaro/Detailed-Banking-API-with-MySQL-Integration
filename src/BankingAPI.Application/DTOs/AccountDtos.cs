namespace BankingAPI.Application.DTOs;

public record AccountResponse(
    Guid Id,
    string AccountNumber,
    decimal Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
