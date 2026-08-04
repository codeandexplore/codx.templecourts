using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class AnswerThread
{
    public Guid Id { get; private set; }
    public Guid StudentAnswerId { get; private set; }
    public AnswerThreadStatus Status { get; private set; }
    public DateTimeOffset? LockedAt { get; private set; }

    public StudentAnswer StudentAnswer { get; private set; } = null!;
    public ICollection<ThreadMessage> Messages { get; private set; } = new List<ThreadMessage>();

    private AnswerThread() { }

    public static AnswerThread Create(Guid studentAnswerId)
    {
        return new AnswerThread
        {
            Id = Guid.NewGuid(),
            StudentAnswerId = studentAnswerId,
            Status = AnswerThreadStatus.Open
        };
    }

    public void Lock()
    {
        if (Status == AnswerThreadStatus.Locked)
            return;

        Status = AnswerThreadStatus.Locked;
        LockedAt = DateTimeOffset.UtcNow;
    }
}
