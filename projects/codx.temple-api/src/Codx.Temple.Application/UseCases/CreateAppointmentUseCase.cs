using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CreateAppointmentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IEmailService _email;

    public CreateAppointmentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser, IEmailService email)
    {
        _db = db;
        _currentUser = currentUser;
        _email = email;
    }

    public virtual async Task<AppointmentDto> ExecuteAsync(CreateAppointmentRequest request, CancellationToken ct = default)
    {
        var student = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.StudentId, ct)
            ?? throw new NotFoundException(nameof(User), request.StudentId);

        var isAssigned = await _db.TeacherAssignments
            .AnyAsync(a => a.StudentId == request.StudentId
                && a.PrimaryTeacherId == _currentUser.UserId
                && a.Status == TeacherAssignmentStatus.Active, ct);

        if (!isAssigned)
            throw new ForbiddenException("Student is not assigned to this teacher");

        var appt = StudySchedule.Create(request.StudentId, _currentUser.UserId, request.ScheduledAt, request.DurationMinutes, request.MeetingLink, _currentUser.UserId);
        _db.StudySchedules.Add(appt);

        var notification = Notification.Create(
            request.StudentId,
            NotificationType.AppointmentCreated,
            "StudySchedule", appt.Id,
            DeliveryChannel.InApp);
        _db.Notifications.Add(notification);

        await _db.SaveChangesAsync(ct);

        _ = _email.SendAsync(student.Email, "New Study Session Scheduled", $"A session has been scheduled for {request.ScheduledAt:g}", ct);

        return new AppointmentDto(appt.Id, appt.StudentId, appt.TeacherId, appt.ScheduledAt, appt.DurationMinutes, appt.MeetingLink, appt.Status.ToString(), appt.CreatedAt);
    }
}
