using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class CommunicationTests
{
    [Fact]
    public void TeacherCheckQuestion_Create_ShouldSetProperties()
    {
        var q = TeacherCheckQuestion.Create(Guid.NewGuid(), Guid.NewGuid(), "test");
        Assert.NotEqual(Guid.Empty, q.Id);
        Assert.Equal("test", q.NoteText);
    }

    [Fact]
    public void AnswerThread_Create_ShouldBeOpen()
    {
        var t = AnswerThread.Create(Guid.NewGuid());
        Assert.Equal(AnswerThreadStatus.Open, t.Status);
    }

    [Fact]
    public void AnswerThread_Lock_ShouldSetLocked()
    {
        var t = AnswerThread.Create(Guid.NewGuid());
        t.Lock();
        Assert.Equal(AnswerThreadStatus.Locked, t.Status);
        Assert.NotNull(t.LockedAt);
    }

    [Fact]
    public void ThreadMessage_Create_ShouldSetProperties()
    {
        var m = ThreadMessage.Create(Guid.NewGuid(), Guid.NewGuid(), "hello");
        Assert.Equal("hello", m.BodyText);
        Assert.True(m.CreatedAt <= DateTimeOffset.UtcNow);
    }
}
