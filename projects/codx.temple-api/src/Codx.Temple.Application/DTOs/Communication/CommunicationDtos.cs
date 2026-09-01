namespace Codx.Temple.Application.DTOs.Communication;

public record TeacherCheckQuestionDto(Guid Id, Guid QuestionKey, string NoteText, bool IsOrphaned, DateTimeOffset CreatedAt);
public record CreateCheckQuestionRequest(Guid QuestionKey, string NoteText);
public record UpdateCheckQuestionRequest(string NoteText);

public record AnswerThreadDto(Guid Id, Guid StudentAnswerId, string Status, DateTimeOffset? LockedAt, List<ThreadMessageDto> Messages);
public record ThreadMessageDto(Guid Id, Guid AuthorId, string AuthorDisplayName, string BodyText, Guid? SourceCheckQuestionId, DateTimeOffset CreatedAt);
public record PostMessageRequest(string BodyText, Guid? SourceCheckQuestionId);

public record PublishedQuestionDto(Guid QuestionKey, string PromptText, int LessonNumber, string LessonTitle);
