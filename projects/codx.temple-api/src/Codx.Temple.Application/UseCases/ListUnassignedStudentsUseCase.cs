using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListUnassignedStudentsUseCase
{
    private readonly IAppDbContext _db;

    public ListUnassignedStudentsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<UserDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var assignedStudentIds = await _db.TeacherAssignments
            .Where(a => a.Status == TeacherAssignmentStatus.Active)
            .Select(a => a.StudentId)
            .ToListAsync(cancellationToken);

        var students = await _db.Users
            .Where(u => u.RoleAssignments.Any(r => r.Role == Role.Student))
            .Where(u => !assignedStudentIds.Contains(u.Id))
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return students.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.DisplayName,
            u.Status.ToString(),
            u.RoleAssignments.Select(r => r.Role.ToString()).ToList())).ToList();
    }
}
