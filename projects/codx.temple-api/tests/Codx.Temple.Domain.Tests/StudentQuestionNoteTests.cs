using Codx.Temple.Domain.Entities;

namespace Codx.Temple.Domain.Tests;

public class StudentQuestionNoteTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var studentId = Guid.NewGuid();
        var questionKey = Guid.NewGuid();

        var note = StudentQuestionNote.Create(studentId, questionKey, "My note");

        Assert.NotEqual(Guid.Empty, note.Id);
        Assert.Equal(studentId, note.StudentId);
        Assert.Equal(questionKey, note.QuestionKey);
        Assert.Equal("My note", note.NoteText);
        Assert.True(note.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void UpdateNoteText_Should_ChangeText()
    {
        var note = StudentQuestionNote.Create(Guid.NewGuid(), Guid.NewGuid(), "original");

        note.UpdateNoteText("updated");

        Assert.Equal("updated", note.NoteText);
    }
}
