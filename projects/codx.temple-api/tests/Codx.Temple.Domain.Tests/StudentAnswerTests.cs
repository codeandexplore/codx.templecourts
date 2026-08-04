using System.Text.Json;
using Codx.Temple.Domain.Entities;

namespace Codx.Temple.Domain.Tests;

public class StudentAnswerTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var questionKey = Guid.NewGuid();
        using var answerDoc = JsonDocument.Parse("\"test answer\"");

        var answer = StudentAnswer.Create(studentId, attemptId, questionKey, answerDoc, "What is love?", "Essay");

        Assert.NotEqual(Guid.Empty, answer.Id);
        Assert.Equal(studentId, answer.StudentId);
        Assert.Equal(attemptId, answer.LessonAttemptId);
        Assert.Equal(questionKey, answer.QuestionKey);
        Assert.Equal("What is love?", answer.PromptSnapshot);
        Assert.Equal("Essay", answer.QuestionTypeSnapshot);
        Assert.True(answer.SubmittedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void UpdateAnswer_Should_UpdateValueAndTimestamp()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var questionKey = Guid.NewGuid();
        using var originalDoc = JsonDocument.Parse("\"original\"");
        var answer = StudentAnswer.Create(studentId, attemptId, questionKey, originalDoc, "prompt", "Essay");
        var originalSubmittedAt = answer.SubmittedAt;

        using var newDoc = JsonDocument.Parse("\"updated\"");
        answer.UpdateAnswer(newDoc);

        Assert.Equal("updated", answer.AnswerValue.RootElement.GetString());
        Assert.True(answer.SubmittedAt > originalSubmittedAt);
    }
}
