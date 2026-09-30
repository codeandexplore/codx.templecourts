using Codx.Temple.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class MarkAllNotificationsReadUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public MarkAllNotificationsReadUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task ExecuteAsync(CancellationToken ct = default)
    {
        var unread = await _db.Notifications
            .Where(n => n.RecipientId == _currentUser.UserId && !n.ReadAt.HasValue)
            .ToListAsync(ct);

        foreach (var n in unread)
            n.MarkRead();

        if (unread.Count > 0)
            await _db.SaveChangesAsync(ct);
    }
}