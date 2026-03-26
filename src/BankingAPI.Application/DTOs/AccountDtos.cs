namespace BankingAPI.Application.DTOs;

// No Id exposed
public record AccountResponse(
    string AccountNumber,
    decimal Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
