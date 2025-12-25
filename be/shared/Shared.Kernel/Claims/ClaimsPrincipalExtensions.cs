using System.Security.Claims;

namespace Shared.Kernel.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(id))
            throw new Exception("UserId claim not found");

        return Guid.Parse(id);
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        var name =
            user.FindFirstValue(ClaimTypes.Name) ??          // ✅ chuẩn nhất
            user.FindFirstValue("unique_name") ??            // ✅ hay có trong JWT
            user.FindFirstValue("name") ??
            user.FindFirstValue("username") ??
            user.Identity?.Name;

        return string.IsNullOrWhiteSpace(name) ? "unknown" : name;
    }
}
