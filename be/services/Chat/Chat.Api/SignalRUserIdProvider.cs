using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chat.Api;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? connection.User?.FindFirstValue("sub");
}
