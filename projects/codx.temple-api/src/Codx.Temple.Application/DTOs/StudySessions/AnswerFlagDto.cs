namespace Codx.Temple.Application.DTOs.StudySessions;

public record AnswerFlagDto(
    Guid Id,
    Guid StudentId,
    Guid QuestionId,
    Guid QuestionKey,
    Guid LessonAttemptId,
    string FlagType,
    Guid RaisedInSessionId,
    DateTimeOffset? ResolvedAt
);
