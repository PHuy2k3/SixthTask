using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

[Authorize]
public class CallHub : Hub
{
    public async Task CallUser(string userId, object offer)
        => await Clients.User(userId).SendAsync("call:incoming", offer);

    public async Task AnswerCall(string callerId, object answer)
        => await Clients.User(callerId).SendAsync("call:answer", answer);

    public async Task IceCandidate(string userId, object candidate)
        => await Clients.User(userId).SendAsync("call:ice", candidate);

    public async Task EndCall(string userId)
        => await Clients.User(userId).SendAsync("call:end");
}
