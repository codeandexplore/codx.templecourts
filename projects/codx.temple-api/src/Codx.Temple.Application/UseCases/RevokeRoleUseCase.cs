using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class RevokeRoleUseCase
{
    private readonly IAppDbContext _db;

    public RevokeRoleUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task ExecuteAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _db.RoleAssignments
            .FirstOrDefaultAsync(r => r.Id == assignmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.RoleAssignment), assignmentId);

        _db.RoleAssignments.Remove(assignment);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
