using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListTeacherAssignmentsUseCase
{
    private readonly IAppDbContext _db;

    public ListTeacherAssignmentsUseCase(IAppDbContext db)
    {
        _db = db;
    }

    public virtual async Task<List<TeacherAssignmentDto>> ExecuteAsync(string? statusFilter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Domain.Entities.TeacherAssignment> query = _db.TeacherAssignments;

        if (!string.IsNullOrEmpty(statusFilter))
        {
            query = query.Where(a => a.Status.ToString() == statusFilter);
        }

        return await query
            .Include(a => a.Student)
            .Include(a => a.PrimaryTeacher)
            .OrderByDescending(a => a.AssignedAt)
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
