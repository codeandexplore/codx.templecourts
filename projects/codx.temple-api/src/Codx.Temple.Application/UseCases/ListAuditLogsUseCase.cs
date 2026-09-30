using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListAuditLogsUseCase
{
    private readonly IAppDbContext _db;

    public ListAuditLogsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<AuditLogDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var logs = await _db.AuditLogs
            .Include(a => a.PerformedBy)
            .OrderByDescending(a => a.CreatedAt)
            .Take(200)
            .ToListAsync(ct);

        var userIds = logs
            .SelectMany(a => new[] { a.PerformedById, a.TargetUserId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var names = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.DisplayName })
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, ct);

        return logs.Select(a => new AuditLogDto(
            a.Id,
            a.Action,
            a.PerformedById,
            names.GetValueOrDefault(a.PerformedById) ?? "",
            a.TargetUserId,
            a.TargetUserId.HasValue ? names.GetValueOrDefault(a.TargetUserId.Value) : null,
            a.Metadata,
            a.CreatedAt)).ToList();
    }
}