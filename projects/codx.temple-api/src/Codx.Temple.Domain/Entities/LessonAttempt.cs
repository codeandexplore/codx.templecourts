using System.Text.Json;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class LessonAttempt
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid LessonKey { get; private set; }
    public Guid LessonVersionId { get; private set; }
    public LessonAttemptStatus Status { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public User Student { get; private set; } = null!;
    public LessonVersion LessonVersion { get; private set; } = null!;
    public ICollection<StudentAnswer> Answers { get; private set; } = new List<StudentAnswer>();

    private LessonAttempt() { }

    public static LessonAttempt Create(
        Guid studentId,
        Guid lessonKey,
        Guid lessonVersionId)
    {
        return new LessonAttempt
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            LessonKey = lessonKey,
            LessonVersionId = lessonVersionId,
            Status = LessonAttemptStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow
        };
    }

    public void Complete()
    {
        if (Status == LessonAttemptStatus.Completed)
            throw new InvalidOperationException("Attempt is already completed");

        Status = LessonAttemptStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
