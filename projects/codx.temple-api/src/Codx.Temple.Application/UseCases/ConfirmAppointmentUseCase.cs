using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ConfirmAppointmentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IEmailService _email;

    public ConfirmAppointmentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser, IEmailService email)
    {
        _db = db;
        _currentUser = currentUser;
        _email = email;
    }

    public virtual async Task<AppointmentDto> ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await _db.StudySchedules
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
            ?? throw new NotFoundException(nameof(StudySchedule), appointmentId);

        if (appt.TeacherId != _currentUser.UserId)
            throw new ForbiddenException("Only the teacher can confirm this appointment");

        appt.Confirm();
        await _db.SaveChangesAsync(ct);

        var notification = Notification.Create(
            appt.StudentId,
            NotificationType.AppointmentCreated,
            "StudySchedule", appt.Id,
            DeliveryChannel.InApp);
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        _ = _email.SendAsync(appt.Student.Email, "Study Session Confirmed", $"Your study session on {appt.ScheduledAt:g} has been confirmed.", ct);

        return MapToDto(appt);
    }

    private static AppointmentDto MapToDto(StudySchedule a) =>
        new(a.Id, a.StudentId, a.TeacherId, a.ScheduledAt, a.DurationMinutes, a.MeetingLink, a.Status.ToString(), a.CreatedAt);
}