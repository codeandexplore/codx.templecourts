using System.Text.Json;

namespace Codx.Temple.Application.DTOs.Lessons;

public record CreateLessonRequest(int Number, string Title);

public record UpdateLessonRequest(int Number, string Title);

public record LessonDto(Guid Id, Guid Key, int Number, string Title, string Status, Guid? CurrentPublishedVersionId);

public record CreateLessonVersionRequest(string? ChangeNotes = null);

public record LessonVersionDto(
    Guid Id,
    Guid LessonId,
    int VersionNumber,
    string Status,
    string? ChangeNotes,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    List<LessonNodeDto> Nodes);

public record CreateLessonNodeRequest(
    Guid? ParentNodeKey,
    string Title,
    string Description,
    bool RequiresPriorSiblingAnswered = false,
    int? Order = null);

public record UpdateLessonNodeRequest(
    string Title,
    string Description,
    bool RequiresPriorSiblingAnswered);

public record ReorderNodesRequest(Guid? ParentNodeKey, List<Guid> OrderedKeys);

public record LessonNodeDto(
    Guid Id,
    Guid Key,
    Guid LessonVersionId,
    Guid? ParentNodeId,
    int Depth,
    int Order,
    string Title,
    string Description,
    bool RequiresPriorSiblingAnswered,
    List<LessonNodeDto> Children,
    List<QuestionDto> Questions);

public record CreateQuestionRequest(
    string QuestionType,
    string PromptText,
    JsonDocument? Metadata = null,
    JsonDocument? ReferenceContext = null,
    int? Order = null);

public record UpdateQuestionRequest(
    string PromptText,
    JsonDocument? Metadata = null,
    JsonDocument? ReferenceContext = null);

public record ReorderQuestionsRequest(List<Guid> OrderedKeys);

public record QuestionDto(
    Guid Id,
    Guid Key,
    Guid LessonNodeId,
    int Order,
    string QuestionType,
    string PromptText,
    JsonDocument? Metadata,
    JsonDocument? ReferenceContext);
