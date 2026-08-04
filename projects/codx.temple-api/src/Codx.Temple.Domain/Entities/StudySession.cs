using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class StudySession
{
    public Guid Id { get; private set; }
    public Guid LessonAttemptId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Guid? StartQuestionId { get; private set; }
    public Guid? EndQuestionId { get; private set; }
    public Guid? CurrentQuestionId { get; private set; }
    public StudySessionStatus Status { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }

    public LessonAttempt LessonAttempt { get; private set; } = null!;

    private StudySession() { }

    public static StudySession Create(
        Guid lessonAttemptId,
        int sequenceNumber,
        Guid? startQuestionId,
        Guid? currentQuestionId)
    {
        return new StudySession
        {
            Id = Guid.NewGuid(),
            LessonAttemptId = lessonAttemptId,
            SequenceNumber = sequenceNumber,
            StartQuestionId = startQuestionId,
            CurrentQuestionId = currentQuestionId,
            Status = StudySessionStatus.NotStarted
        };
    }

    public void Start()
    {
        if (Status != StudySessionStatus.NotStarted)
            throw new InvalidOperationException("Session is not in NotStarted state");

        Status = StudySessionStatus.InProgress;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public void Advance(Guid currentQuestionId)
    {
        if (Status != StudySessionStatus.InProgress)
            throw new InvalidOperationException("Session is not in progress");

        CurrentQuestionId = currentQuestionId;
    }

    public void Complete()
    {
        if (Status != StudySessionStatus.InProgress)
            throw new InvalidOperationException("Session is not in progress");

        Status = StudySessionStatus.Completed;
        EndQuestionId = CurrentQuestionId;
        EndedAt = DateTimeOffset.UtcNow;
    }
}
