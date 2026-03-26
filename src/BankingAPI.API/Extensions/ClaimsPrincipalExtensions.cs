using System.Security.Claims;

namespace BankingAPI.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? principal.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }
}
