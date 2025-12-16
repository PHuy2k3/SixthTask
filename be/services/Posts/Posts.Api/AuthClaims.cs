using System.Security.Claims;

namespace Posts.Api;

public static class AuthClaims
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(sub!);
    }

    public static string GetUserName(this ClaimsPrincipal user)
        => user.FindFirstValue("username") ?? "";
}
