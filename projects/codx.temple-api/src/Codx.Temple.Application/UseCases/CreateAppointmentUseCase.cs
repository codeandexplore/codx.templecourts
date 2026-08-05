using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
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

        var appt = StudySchedule.Create(request.StudentId, _currentUser.UserId, request.ScheduledAt, request.DurationMinutes, request.MeetingLink, _currentUser.UserId);
        _db.StudySchedules.Add(appt);
        await _db.SaveChangesAsync(ct);

        _ = _email.SendAsync(student.Email, "New Study Session Scheduled", $"A session has been scheduled for {request.ScheduledAt:g}", ct);

        return new AppointmentDto(appt.Id, appt.StudentId, appt.TeacherId, appt.ScheduledAt, appt.DurationMinutes, appt.MeetingLink, appt.Status.ToString(), appt.CreatedAt);
    }
}
