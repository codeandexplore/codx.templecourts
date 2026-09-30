using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class CancelAppointmentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public CancelAppointmentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<AppointmentDto> ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await _db.StudySchedules
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
            ?? throw new NotFoundException(nameof(StudySchedule), appointmentId);

        if (appt.TeacherId != _currentUser.UserId)
            throw new ForbiddenException("Only the teacher can cancel this appointment");

        appt.Cancel();
        await _db.SaveChangesAsync(ct);

        return new AppointmentDto(appt.Id, appt.StudentId, appt.TeacherId, appt.ScheduledAt, appt.DurationMinutes, appt.MeetingLink, appt.Status.ToString(), appt.CreatedAt);
    }
}