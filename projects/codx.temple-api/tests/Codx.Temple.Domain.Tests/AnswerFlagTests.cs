using Codx.Temple.Domain.Entities;

namespace Codx.Temple.Domain.Tests;

public class AnswerFlagTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var studentId = Guid.NewGuid();
        var qId = Guid.NewGuid();
        var qKey = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var flag = AnswerFlag.Create(studentId, qId, qKey, attemptId, sessionId);

        Assert.NotEqual(Guid.Empty, flag.Id);
        Assert.Equal(studentId, flag.StudentId);
        Assert.Equal(qKey, flag.QuestionKey);
        Assert.Equal(sessionId, flag.RaisedInSessionId);
        Assert.Null(flag.ResolvedAt);
    }

    [Fact]
    public void Resolve_Should_SetResolvedAt()
    {
        var flag = AnswerFlag.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        flag.Resolve();

        Assert.NotNull(flag.ResolvedAt);
    }

    [Fact]
    public void Resolve_Should_ThrowWhenAlreadyResolved()
    {
        var flag = AnswerFlag.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        flag.Resolve();

        Assert.Throws<InvalidOperationException>(() => flag.Resolve());
    }
}
