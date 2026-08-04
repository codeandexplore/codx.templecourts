using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class ListAppointmentsUseCase
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserAccessor _currentUser;

    public ListAppointmentsUseCase(IAppDbContext db, ICurrentUserAccessor currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public virtual async Task<List<AppointmentDto>> ExecuteAsync(CancellationToken ct = default)
    {
        return await _db.StudySchedules
            .Where(s => s.StudentId == _currentUser.UserId || s.TeacherId == _currentUser.UserId)
            .OrderBy(s => s.ScheduledAt)
            .Select(s => new AppointmentDto(s.Id, s.StudentId, s.TeacherId, s.ScheduledAt, s.DurationMinutes, s.MeetingLink, s.Status.ToString(), s.CreatedAt))
            .ToListAsync(ct);
    }
}
