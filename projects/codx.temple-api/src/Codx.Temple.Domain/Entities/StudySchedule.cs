using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class StudySchedule
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid TeacherId { get; private set; }
    public DateTimeOffset ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? MeetingLink { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public Guid CreatedById { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User Student { get; private set; } = null!;
    public User Teacher { get; private set; } = null!;

    private StudySchedule() { }

    public static StudySchedule Create(Guid studentId, Guid teacherId, DateTimeOffset scheduledAt, int durationMinutes, string? meetingLink, Guid createdById)
    {
        return new StudySchedule
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            TeacherId = teacherId,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            MeetingLink = meetingLink,
            Status = AppointmentStatus.Proposed,
            CreatedById = createdById,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Proposed)
            throw new InvalidOperationException("Only Proposed appointments can be confirmed");
        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a completed or already cancelled appointment");
        Status = AppointmentStatus.Cancelled;
    }
}
