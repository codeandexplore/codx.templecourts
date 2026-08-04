namespace Codx.Temple.Application.DTOs.StudentNotes;

public record StudentQuestionNoteDto(
    Guid Id,
    Guid QuestionKey,
    string NoteText,
    DateTimeOffset CreatedAt
);

public record CreateNoteRequest(
    string NoteText
);

public record UpdateNoteRequest(
    string NoteText
);
