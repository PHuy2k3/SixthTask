using System.Security.Claims;

namespace Shared.Kernel.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(id))
            throw new Exception("UserId claim not found");

        return Guid.Parse(id);
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        return
            user.FindFirst(ClaimTypes.Name)?.Value ??
            user.FindFirst("name")?.Value ??
            "unknown";
    }
}