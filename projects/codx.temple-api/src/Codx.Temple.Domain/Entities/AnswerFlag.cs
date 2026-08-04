using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class AnswerFlag
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid QuestionKey { get; private set; }
    public Guid LessonAttemptId { get; private set; }
    public AnswerFlagType FlagType { get; private set; }
    public Guid RaisedInSessionId { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }

    public User Student { get; private set; } = null!;
    public LessonAttempt LessonAttempt { get; private set; } = null!;
    public StudySession RaisedInSession { get; private set; } = null!;

    private AnswerFlag() { }

    public static AnswerFlag Create(
        Guid studentId,
        Guid questionId,
        Guid questionKey,
        Guid lessonAttemptId,
        Guid raisedInSessionId)
    {
        return new AnswerFlag
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            QuestionId = questionId,
            QuestionKey = questionKey,
            LessonAttemptId = lessonAttemptId,
            FlagType = AnswerFlagType.UnansweredAtReview,
            RaisedInSessionId = raisedInSessionId
        };
    }

    public void Resolve()
    {
        if (ResolvedAt.HasValue)
            throw new InvalidOperationException("Flag is already resolved");

        ResolvedAt = DateTimeOffset.UtcNow;
    }
}
