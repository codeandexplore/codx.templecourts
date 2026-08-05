namespace Codx.Temple.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public string Action { get; private set; } = null!;
    public Guid PerformedById { get; private set; }
    public Guid? TargetUserId { get; private set; }
    public string? Metadata { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User PerformedBy { get; private set; } = null!;

    private AuditLog() { }

    public static AuditLog Create(string action, Guid performedById, Guid? targetUserId = null, string? metadata = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            PerformedById = performedById,
            TargetUserId = targetUserId,
            Metadata = metadata,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
