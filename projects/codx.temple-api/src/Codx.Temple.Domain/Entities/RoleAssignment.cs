using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class RoleAssignment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Role Role { get; private set; }
    public Guid AssignedBy { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }

    public User User { get; private set; } = null!;
    public User Assigner { get; private set; } = null!;

    private RoleAssignment() { }

    public static RoleAssignment Create(Guid userId, Role role, Guid assignedBy)
    {
        return new RoleAssignment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Role = role,
            AssignedBy = assignedBy,
            AssignedAt = DateTimeOffset.UtcNow
        };
    }
}
