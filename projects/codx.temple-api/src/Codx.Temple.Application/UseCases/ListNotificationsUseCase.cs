using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListNotificationsUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListNotificationsUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<NotificationDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var items = await _db.Notifications
            .Where(n => n.RecipientId == _currentUser.UserId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

        return items.Select(n => new NotificationDto(
            n.Id,
            n.Type.ToString(),
            n.ReferenceType,
            n.ReferenceId,
            n.DeliveryChannel.ToString(),
            n.ReadAt.HasValue,
            n.CreatedAt)).ToList();
    }
}