using BankingAPI.Application.DTOs;
using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime AccessTokenExpiresAt { get; }
}
