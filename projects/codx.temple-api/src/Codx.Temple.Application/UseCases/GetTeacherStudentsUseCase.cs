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
        var assignments = await _db.TeacherAssignments
            .Where(a => a.PrimaryTeacherId == _currentUser.UserId && a.Status == TeacherAssignmentStatus.Active)
            .Include(a => a.Student)
            .Include(a => a.PrimaryTeacher)
            .ToListAsync(cancellationToken);

        var studentIds = assignments.Select(a => a.StudentId).ToList();
        var latestAttempts = await _db.LessonAttempts
            .Where(a => studentIds.Contains(a.StudentId) && a.Status == LessonAttemptStatus.InProgress)
            .GroupBy(a => a.StudentId)
            .Select(g => g.OrderByDescending(a => a.StartedAt).First())
            .ToListAsync(cancellationToken);

        var attemptMap = latestAttempts.ToDictionary(a => a.StudentId, a => (Guid?)a.Id);

        return assignments.Select(a => new TeacherAssignmentDto(
            a.Id,
            a.StudentId,
            a.Student.Email,
            a.Student.DisplayName,
            a.PrimaryTeacherId,
            a.PrimaryTeacher.Email,
            a.PrimaryTeacher.DisplayName,
            a.Status.ToString(),
            a.AssignedAt,
            a.EndedAt,
            attemptMap.GetValueOrDefault(a.StudentId))).ToList();
    }
}
