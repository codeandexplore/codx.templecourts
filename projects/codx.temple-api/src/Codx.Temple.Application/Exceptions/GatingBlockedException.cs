namespace Codx.Temple.Application.Exceptions;

public sealed class GatingBlockedException : Exception
{
    public IReadOnlyCollection<Guid> RequiredQuestionKeys { get; }

    public GatingBlockedException(string message, IReadOnlyCollection<Guid> requiredQuestionKeys)
        : base(message)
    {
        RequiredQuestionKeys = requiredQuestionKeys;
    }
}
