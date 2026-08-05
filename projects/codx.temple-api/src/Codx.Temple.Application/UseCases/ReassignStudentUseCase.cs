using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ReassignStudentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ReassignStudentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<TeacherAssignmentDto> ExecuteAsync(ReassignStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.NewTeacherId == request.StudentId)
            throw new InvalidOperationException("Student cannot be assigned to themselves");

        var current = await _db.TeacherAssignments
            .FirstOrDefaultAsync(a => a.StudentId == request.StudentId && a.Status == TeacherAssignmentStatus.Active, cancellationToken)
            ?? throw new NotFoundException("TeacherAssignment", $"No active assignment for student {request.StudentId}");

        if (current.PrimaryTeacherId == request.NewTeacherId)
            throw new InvalidOperationException("Student is already assigned to this teacher");

        var newTeacher = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.NewTeacherId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.NewTeacherId);

        current.End();

        var assignment = TeacherAssignment.Create(request.StudentId, request.NewTeacherId, _currentUser.UserId);
        _db.TeacherAssignments.Add(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        var audit = AuditLog.Create("StudentReassigned", _currentUser.UserId, request.StudentId,
            $"Reassigned from teacher {current.PrimaryTeacherId} to {request.NewTeacherId}");
        _db.AuditLogs.Add(audit);
        await _db.SaveChangesAsync(cancellationToken);

        var student = await _db.Users.FirstAsync(u => u.Id == request.StudentId, cancellationToken);

        return new TeacherAssignmentDto(
            assignment.Id,
            request.StudentId,
            student.Email,
            student.DisplayName,
            request.NewTeacherId,
            newTeacher.Email,
            newTeacher.DisplayName,
            assignment.Status.ToString(),
            assignment.AssignedAt,
            assignment.EndedAt);
    }
}
