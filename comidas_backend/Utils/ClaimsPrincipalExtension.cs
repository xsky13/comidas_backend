using System.Security.Claims;

namespace comidas_backend.Utils;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new BadHttpRequestException("Usuario no logueado");
        }

        return userId;
    }
}