using System.Security.Claims;

namespace Shared.Kernel;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id =
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub"); // fallback JWT standard

        if (string.IsNullOrWhiteSpace(id))
            throw new Exception("UserId not found in token");

        return Guid.Parse(id);
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        return
            user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("username")
            ?? user.Identity?.Name
            ?? "unknown";
    }
}
