using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListRoleAssignmentsUseCase
{
    private readonly IAppDbContext _db;

    public ListRoleAssignmentsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<RoleAssignmentDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _db.RoleAssignments
            .Include(r => r.User)
            .Select(r => new RoleAssignmentDto(
                r.Id,
                r.UserId,
                r.User.Email,
                r.User.DisplayName,
                r.Role.ToString(),
                r.AssignedBy,
                r.AssignedAt))
            .ToListAsync(cancellationToken);
    }
}
