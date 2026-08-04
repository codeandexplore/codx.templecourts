using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ClaimStudentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ClaimStudentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<TeacherAssignmentDto> ExecuteAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await _db.Users.FirstOrDefaultAsync(u => u.Id == studentId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), studentId);

        var existing = await _db.TeacherAssignments
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Status == TeacherAssignmentStatus.Active, cancellationToken);

        if (existing is not null)
            throw new ConflictException("Student already has an active Teacher assignment");

        var assignment = TeacherAssignment.Create(studentId, _currentUser.UserId, _currentUser.UserId);
        _db.TeacherAssignments.Add(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        return new TeacherAssignmentDto(
            assignment.Id,
            studentId,
            student.Email,
            student.DisplayName,
            _currentUser.UserId,
            _currentUser.Email ?? "",
            _currentUser.DisplayName ?? "",
            assignment.Status.ToString(),
            assignment.AssignedAt,
            assignment.EndedAt);
    }
}
