using Codx.Temple.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.API.Hubs;

[Authorize]
public class StudySessionHub : Hub
{
    private readonly Infrastructure.Data.AppDbContext _db;

    public StudySessionHub(Infrastructure.Data.AppDbContext db)
    {
        _db = db;
    }

    public async Task JoinSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new HubException("Session ID is required");

        if (!Guid.TryParse(sessionId, out var sid))
            throw new HubException("Invalid session ID format");

        await AuthorizeSessionAccess(sid);
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
        if (!Guid.TryParse(sessionId, out var sid))
            throw new HubException("Invalid session ID format");

        await AuthorizeSessionAccess(sid);
        await Clients.Group(sessionId).SendAsync("SessionStateUpdated", state);
    }

    public async Task JoinThread(string threadId)
    {
        if (string.IsNullOrEmpty(threadId) || !Guid.TryParse(threadId, out var tid))
            throw new HubException("Invalid thread ID format");

        await AuthorizeThreadAccess(tid);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"thread:{threadId}");
        await Clients.Caller.SendAsync("ThreadJoined", threadId);
    }

    public async Task LeaveThread(string threadId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread:{threadId}");
        await Clients.Caller.SendAsync("ThreadLeft", threadId);
    }

    private async Task AuthorizeThreadAccess(Guid threadId)
    {
        var thread = await _db.AnswerThreads
            .Include(t => t.StudentAnswer)
            .FirstOrDefaultAsync(t => t.Id == threadId)
            ?? throw new HubException("Thread not found");

        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new HubException("Unauthorized");

        if (thread.StudentAnswer.StudentId == userId)
            return;

        var isTeacher = await _db.TeacherAssignments
            .AnyAsync(a => a.StudentId == thread.StudentAnswer.StudentId
                && a.PrimaryTeacherId == userId
                && a.Status == TeacherAssignmentStatus.Active);

        if (!isTeacher)
            throw new HubException("Not authorized for this thread");
    }

    private async Task AuthorizeSessionAccess(Guid sessionId)
    {
        var session = await _db.StudySessions
            .Include(s => s.LessonAttempt)
            .FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new HubException("Session not found");

        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new HubException("Unauthorized");

        if (session.LessonAttempt.StudentId == userId)
            return;

        var isTeacher = await _db.TeacherAssignments
            .AnyAsync(a => a.StudentId == session.LessonAttempt.StudentId
                && a.PrimaryTeacherId == userId
                && a.Status == TeacherAssignmentStatus.Active);

        if (!isTeacher)
            throw new HubException("Not authorized for this session");
    }
}
