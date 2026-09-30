using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class RevokeRoleUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public RevokeRoleUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task ExecuteAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _db.RoleAssignments
            .FirstOrDefaultAsync(r => r.Id == assignmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(RoleAssignment), assignmentId);

        _db.RoleAssignments.Remove(assignment);

        var audit = AuditLog.Create("RoleRevoked", _currentUser.UserId, assignment.UserId,
            $"Revoked role: {assignment.Role}");
        _db.AuditLogs.Add(audit);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
