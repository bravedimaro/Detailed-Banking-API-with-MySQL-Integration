namespace BankingAPI.Application.DTOs;

public record RegisterRequest(string FullName, string Email, string Password);

public record LoginRequest(string Email, string Password);

// Login response — access + refresh tokens only
public record LoginResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);

// Refresh token request/response
public record RefreshTokenRequest(string RefreshToken);

public record UpdateProfileRequest(string? FullName, string? PhoneNumber, string? Address);

// No Id exposed
public record UserProfileResponse(
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    DateTime CreatedAt
);
