using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class LessonAttemptTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var studentId = Guid.NewGuid();
        var lessonKey = Guid.NewGuid();
        var versionId = Guid.NewGuid();

        var attempt = LessonAttempt.Create(studentId, lessonKey, versionId);

        Assert.NotEqual(Guid.Empty, attempt.Id);
        Assert.Equal(studentId, attempt.StudentId);
        Assert.Equal(lessonKey, attempt.LessonKey);
        Assert.Equal(versionId, attempt.LessonVersionId);
        Assert.Equal(LessonAttemptStatus.InProgress, attempt.Status);
        Assert.True(attempt.StartedAt <= DateTimeOffset.UtcNow);
        Assert.Null(attempt.CompletedAt);
        Assert.NotNull(attempt.Answers);
        Assert.Empty(attempt.Answers);
    }

    [Fact]
    public void Complete_Should_SetStatusAndCompletedAt()
    {
        var attempt = LessonAttempt.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        attempt.Complete();

        Assert.Equal(LessonAttemptStatus.Completed, attempt.Status);
        Assert.NotNull(attempt.CompletedAt);
        Assert.True(attempt.CompletedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Complete_Should_ThrowWhenAlreadyCompleted()
    {
        var attempt = LessonAttempt.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        attempt.Complete();

        var ex = Assert.Throws<InvalidOperationException>(() => attempt.Complete());
        Assert.Contains("already completed", ex.Message);
    }
}
