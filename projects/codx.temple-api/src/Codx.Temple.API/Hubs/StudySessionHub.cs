using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Codx.Temple.API.Hubs;

[Authorize]
public class StudySessionHub : Hub
{
    public async Task JoinSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new HubException("Session ID is required");

        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Caller.SendAsync("SessionJoined", sessionId);
    }

    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Caller.SendAsync("SessionLeft", sessionId);
    }

    public async Task BroadcastSessionState(string sessionId, object state)
    {
        await Clients.Group(sessionId).SendAsync("SessionStateUpdated", state);
    }
}
