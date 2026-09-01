namespace Codx.Temple.Application.DTOs.StudySessions;

public record SessionQuestionsDto(
    Guid SessionId,
    Guid LessonAttemptId,
    string Status,
    Guid? CurrentQuestionId,
    string StudentDisplayName,
    int LessonNumber,
    string LessonTitle,
    List<QuestionWithAnswerDto> Questions
);

public record QuestionWithAnswerDto(
    Guid Key,
    int Order,
    int Depth,
    string ParentNodeTitle,
    string QuestionType,
    string PromptText,
    StudentAnswerInfoDto? Answer,
    bool IsReviewed,
    AnswerFlagInfoDto? Flag,
    Guid? ThreadId = null
);

public record StudentAnswerInfoDto(
    string Value,
    DateTimeOffset SubmittedAt
);

public record AnswerFlagInfoDto(
    string Type,
    DateTimeOffset CreatedAt
);
