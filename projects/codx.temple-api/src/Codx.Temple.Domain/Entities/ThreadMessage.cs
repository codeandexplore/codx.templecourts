namespace Codx.Temple.Domain.Entities;

public class ThreadMessage
{
    public Guid Id { get; private set; }
    public Guid AnswerThreadId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string BodyText { get; private set; } = null!;
    public Guid? SourceCheckQuestionId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public AnswerThread AnswerThread { get; private set; } = null!;
    public User Author { get; private set; } = null!;

    private ThreadMessage() { }

    public static ThreadMessage Create(Guid answerThreadId, Guid authorId, string bodyText, Guid? sourceCheckQuestionId = null)
    {
        return new ThreadMessage
        {
            Id = Guid.NewGuid(),
            AnswerThreadId = answerThreadId,
            AuthorId = authorId,
            BodyText = bodyText,
            SourceCheckQuestionId = sourceCheckQuestionId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
