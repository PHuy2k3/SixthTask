using System.Security.Claims;

namespace Chat.Api;

public static class AuthClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var s = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub")
                ?? throw new Exception("Missing user id claim");
        return Guid.Parse(s);
    }

    public static string GetUserName(this ClaimsPrincipal user)
        => user.Identity?.Name
           ?? user.FindFirstValue(ClaimTypes.Name)
           ?? user.FindFirstValue("name")
           ?? "user";
}
