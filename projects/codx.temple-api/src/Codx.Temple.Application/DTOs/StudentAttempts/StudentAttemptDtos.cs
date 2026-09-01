namespace Codx.Temple.Application.DTOs.StudentAttempts;

public record LessonAttemptDto(
    Guid Id,
    Guid LessonKey,
    Guid LessonVersionId,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    List<Guid> AnsweredQuestionKeys,
    Guid? ActiveSessionId = null
);

public record StudentAnswerDto(
    Guid Id,
    Guid LessonAttemptId,
    Guid QuestionKey,
    object AnswerValue,
    string PromptSnapshot,
    string QuestionTypeSnapshot,
    DateTimeOffset SubmittedAt,
    Guid? ThreadId = null
);

public record SubmitAnswerRequest(
    Guid QuestionKey,
    object AnswerValue
);

public record StudentAttemptSummaryDto(
    Guid Id,
    Guid LessonKey,
    int LessonNumber,
    string LessonTitle,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    int AnsweredCount
);
