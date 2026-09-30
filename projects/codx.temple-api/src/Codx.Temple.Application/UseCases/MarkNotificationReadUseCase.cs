using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class MarkNotificationReadUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public MarkNotificationReadUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task ExecuteAsync(Guid notificationId, CancellationToken ct = default)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId, ct)
            ?? throw new NotFoundException(nameof(Notification), notificationId);

        if (notification.RecipientId != _currentUser.UserId)
            throw new ForbiddenException("Not authorized for this notification");

        notification.MarkRead();
        await _db.SaveChangesAsync(ct);
    }
}