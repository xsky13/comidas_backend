using System.Security.Claims;

namespace comidas_backend.Utils;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}