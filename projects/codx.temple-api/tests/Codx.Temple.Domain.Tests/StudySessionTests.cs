using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class StudySessionTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var attemptId = Guid.NewGuid();
        var startQ = Guid.NewGuid();

        var session = StudySession.Create(attemptId, 1, startQ, startQ);

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(attemptId, session.LessonAttemptId);
        Assert.Equal(1, session.SequenceNumber);
        Assert.Equal(startQ, session.StartQuestionId);
        Assert.Equal(StudySessionStatus.NotStarted, session.Status);
        Assert.Null(session.StartedAt);
    }

    [Fact]
    public void Start_Should_SetInProgress()
    {
        var session = StudySession.Create(Guid.NewGuid(), 1, Guid.NewGuid(), Guid.NewGuid());
        session.Start();

        Assert.Equal(StudySessionStatus.InProgress, session.Status);
        Assert.NotNull(session.StartedAt);
    }

    [Fact]
    public void Start_Should_ThrowWhenAlreadyStarted()
    {
        var session = StudySession.Create(Guid.NewGuid(), 1, Guid.NewGuid(), Guid.NewGuid());
        session.Start();

        Assert.Throws<InvalidOperationException>(() => session.Start());
    }

    [Fact]
    public void Complete_Should_SetCompleted()
    {
        var session = StudySession.Create(Guid.NewGuid(), 1, Guid.NewGuid(), Guid.NewGuid());
        session.Start();
        session.Complete();

        Assert.Equal(StudySessionStatus.Completed, session.Status);
        Assert.NotNull(session.EndedAt);
    }
}
