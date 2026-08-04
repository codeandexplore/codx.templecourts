using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class TeacherAssignment
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid PrimaryTeacherId { get; private set; }
    public Guid AssignedById { get; private set; }
    public TeacherAssignmentStatus Status { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }

    public User Student { get; private set; } = null!;
    public User PrimaryTeacher { get; private set; } = null!;
    public User AssignedBy { get; private set; } = null!;

    private TeacherAssignment() { }

    public static TeacherAssignment Create(
        Guid studentId,
        Guid primaryTeacherId,
        Guid assignedById)
    {
        return new TeacherAssignment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            PrimaryTeacherId = primaryTeacherId,
            AssignedById = assignedById,
            Status = TeacherAssignmentStatus.Active,
            AssignedAt = DateTimeOffset.UtcNow
        };
    }

    public void End()
    {
        if (Status == TeacherAssignmentStatus.Ended)
            throw new InvalidOperationException("Assignment is already ended");

        Status = TeacherAssignmentStatus.Ended;
        EndedAt = DateTimeOffset.UtcNow;
    }
}
