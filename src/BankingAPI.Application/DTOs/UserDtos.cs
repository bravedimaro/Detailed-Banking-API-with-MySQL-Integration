namespace BankingAPI.Application.DTOs;

public record RegisterRequest(string FullName, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, string Email, string FullName, Guid UserId);

public record UpdateProfileRequest(string? FullName, string? PhoneNumber, string? Address);

public record UserProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    DateTime CreatedAt
);
