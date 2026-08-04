namespace Codx.Temple.Application.DTOs.StudySessions;

public record StudySessionDto(
    Guid Id,
    Guid LessonAttemptId,
    int SequenceNumber,
    Guid? StartQuestionId,
    Guid? EndQuestionId,
    Guid? CurrentQuestionId,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? EndedAt
);

public record StartSessionRequest(
    Guid LessonAttemptId
);

public record AdvanceSessionRequest(
    Guid CurrentQuestionId
);

public record MarkReviewedRequest(
    Guid LessonAttemptId,
    Guid QuestionKey
);
