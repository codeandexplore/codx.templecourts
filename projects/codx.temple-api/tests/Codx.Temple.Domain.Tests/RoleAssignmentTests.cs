using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class RoleAssignmentTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var userId = Guid.NewGuid();
        var assignedBy = Guid.NewGuid();

        var assignment = RoleAssignment.Create(userId, Role.Teacher, assignedBy);

        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.Equal(userId, assignment.UserId);
        Assert.Equal(Role.Teacher, assignment.Role);
        Assert.Equal(assignedBy, assignment.AssignedBy);
        Assert.True(assignment.AssignedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(Role.Admin)]
    [InlineData(Role.Teacher)]
    [InlineData(Role.Student)]
    public void Create_Should_SupportAllRoles(Role role)
    {
        var assignment = RoleAssignment.Create(Guid.NewGuid(), role, Guid.NewGuid());
        Assert.Equal(role, assignment.Role);
    }
}
