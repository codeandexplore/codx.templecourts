using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GetAppointmentUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public GetAppointmentUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<AppointmentDto> ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await _db.StudySchedules
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
            ?? throw new NotFoundException(nameof(StudySchedule), appointmentId);

        if (appt.StudentId != _currentUser.UserId && appt.TeacherId != _currentUser.UserId)
            throw new ForbiddenException("Not authorized for this appointment");

        return new AppointmentDto(appt.Id, appt.StudentId, appt.TeacherId, appt.ScheduledAt, appt.DurationMinutes, appt.MeetingLink, appt.Status.ToString(), appt.CreatedAt);
    }
}