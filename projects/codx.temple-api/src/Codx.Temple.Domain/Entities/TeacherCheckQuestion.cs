namespace Codx.Temple.Domain.Entities;

public class TeacherCheckQuestion
{
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid QuestionKey { get; private set; }
    public string NoteText { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public User Teacher { get; private set; } = null!;

    private TeacherCheckQuestion() { }

    public static TeacherCheckQuestion Create(Guid teacherId, Guid questionKey, string noteText)
    {
        return new TeacherCheckQuestion
        {
            Id = Guid.NewGuid(),
            TeacherId = teacherId,
            QuestionKey = questionKey,
            NoteText = noteText,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string noteText)
    {
        NoteText = noteText;
    }
}
