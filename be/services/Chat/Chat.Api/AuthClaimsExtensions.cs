using System.Security.Claims;

namespace Chat.Api;

public static class AuthClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var s = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new Exception("Missing user id claim");
        return Guid.Parse(s);
    }

    public static string GetUserName(this ClaimsPrincipal user)
        => user.Identity?.Name
           ?? user.FindFirst(ClaimTypes.Name)?.Value
           ?? user.FindFirst("name")?.Value
           ?? "user";
}
