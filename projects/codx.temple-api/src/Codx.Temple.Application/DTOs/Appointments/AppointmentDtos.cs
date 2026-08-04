namespace Codx.Temple.Application.DTOs.Appointments;

public record AppointmentDto(Guid Id, Guid StudentId, Guid TeacherId, DateTimeOffset ScheduledAt, int DurationMinutes, string? MeetingLink, string Status, DateTimeOffset CreatedAt);
public record CreateAppointmentRequest(Guid StudentId, DateTimeOffset ScheduledAt, int DurationMinutes, string? MeetingLink);
