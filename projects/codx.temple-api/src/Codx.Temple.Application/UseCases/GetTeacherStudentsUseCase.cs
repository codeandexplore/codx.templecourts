using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetTeacherStudentsUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetTeacherStudentsUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<TeacherAssignmentDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _db.TeacherAssignments
            .Where(a => a.PrimaryTeacherId == _currentUser.UserId && a.Status == TeacherAssignmentStatus.Active)
            .Include(a => a.Student)
            .Include(a => a.PrimaryTeacher)
            .Select(a => new TeacherAssignmentDto(
                a.Id,
                a.StudentId,
                a.Student.Email,
                a.Student.DisplayName,
                a.PrimaryTeacherId,
                a.PrimaryTeacher.Email,
                a.PrimaryTeacher.DisplayName,
                a.Status.ToString(),
                a.AssignedAt,
                a.EndedAt))
            .ToListAsync(cancellationToken);
    }
}
