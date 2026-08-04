namespace Codx.Temple.Domain.Entities;

public class StudentQuestionNote
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid QuestionKey { get; private set; }
    public string NoteText { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public User Student { get; private set; } = null!;

    private StudentQuestionNote() { }

    public static StudentQuestionNote Create(
        Guid studentId,
        Guid questionKey,
        string noteText)
    {
        return new StudentQuestionNote
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            QuestionKey = questionKey,
            NoteText = noteText,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateNoteText(string noteText)
    {
        NoteText = noteText;
    }
}
