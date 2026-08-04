using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class AssignRoleUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public AssignRoleUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<RoleAssignmentDto> ExecuteAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<Role>(request.Role, true, out var role))
            throw new ConflictException($"Invalid role: {request.Role}.");

        var exists = await _db.RoleAssignments
            .AnyAsync(r => r.UserId == request.UserId && r.Role == role, cancellationToken);
        if (exists)
            throw new ConflictException($"User already has the {role} role.");

        var assignment = RoleAssignment.Create(request.UserId, role, _currentUser.UserId);
        _db.RoleAssignments.Add(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == assignment.UserId, cancellationToken);

        return new RoleAssignmentDto(
            assignment.Id,
            assignment.UserId,
            user?.Email ?? "",
            user?.DisplayName ?? "",
            assignment.Role.ToString(),
            assignment.AssignedBy,
            assignment.AssignedAt);
    }
}
