using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class TeacherAssignmentTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var assignedById = Guid.NewGuid();

        var assignment = TeacherAssignment.Create(studentId, teacherId, assignedById);

        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.Equal(studentId, assignment.StudentId);
        Assert.Equal(teacherId, assignment.PrimaryTeacherId);
        Assert.Equal(assignedById, assignment.AssignedById);
        Assert.Equal(TeacherAssignmentStatus.Active, assignment.Status);
        Assert.True(assignment.AssignedAt <= DateTimeOffset.UtcNow);
        Assert.Null(assignment.EndedAt);
    }

    [Fact]
    public void End_Should_SetStatusAndEndedAt()
    {
        var assignment = TeacherAssignment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        assignment.End();

        Assert.Equal(TeacherAssignmentStatus.Ended, assignment.Status);
        Assert.NotNull(assignment.EndedAt);
    }

    [Fact]
    public void End_Should_ThrowWhenAlreadyEnded()
    {
        var assignment = TeacherAssignment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        assignment.End();

        var ex = Assert.Throws<InvalidOperationException>(() => assignment.End());
        Assert.Contains("already ended", ex.Message);
    }
}
