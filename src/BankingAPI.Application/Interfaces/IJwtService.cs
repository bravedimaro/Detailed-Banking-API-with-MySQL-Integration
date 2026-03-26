using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
