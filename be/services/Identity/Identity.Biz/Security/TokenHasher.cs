using System.Security.Cryptography;
using System.Text;

namespace Identity.Biz.Security;

public static class TokenHasher
{
    public static string Sha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    public static string NewRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
}
