using System.Text.Json;

namespace Codx.Temple.Domain.Entities;

public class StudentAnswer
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid LessonAttemptId { get; private set; }
    public Guid QuestionKey { get; private set; }
    public JsonDocument AnswerValue { get; private set; } = null!;
    public string PromptSnapshot { get; private set; } = null!;
    public string QuestionTypeSnapshot { get; private set; } = null!;
    public DateTimeOffset SubmittedAt { get; private set; }

    public User Student { get; private set; } = null!;
    public LessonAttempt LessonAttempt { get; private set; } = null!;

    private StudentAnswer() { }

    public static StudentAnswer Create(
        Guid studentId,
        Guid lessonAttemptId,
        Guid questionKey,
        JsonDocument answerValue,
        string promptSnapshot,
        string questionTypeSnapshot)
    {
        return new StudentAnswer
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            LessonAttemptId = lessonAttemptId,
            QuestionKey = questionKey,
            AnswerValue = answerValue,
            PromptSnapshot = promptSnapshot,
            QuestionTypeSnapshot = questionTypeSnapshot,
            SubmittedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateAnswer(JsonDocument answerValue)
    {
        AnswerValue = answerValue;
        SubmittedAt = DateTimeOffset.UtcNow;
    }
}
