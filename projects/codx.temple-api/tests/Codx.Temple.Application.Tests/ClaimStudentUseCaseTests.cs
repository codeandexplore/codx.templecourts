using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Moq;

namespace Codx.Temple.Application.Tests;

public class ClaimStudentUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<ICurrentUserAccessor> _currentUserMock;
    private readonly ClaimStudentUseCase _useCase;

    public ClaimStudentUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _currentUserMock = new Mock<ICurrentUserAccessor>();
        _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Email).Returns("teacher@test.com");
        _currentUserMock.Setup(x => x.DisplayName).Returns("Teacher");
        _useCase = new ClaimStudentUseCase(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateAssignment()
    {
        var student = User.CreateWithPassword("student@test.com", "hash", "Student");
        var students = new List<User> { student };
        var assignments = new List<TeacherAssignment>().AsQueryable();

        _dbMock.Setup(db => db.Users).Returns(DbSetMockHelper.CreateMockDbSet(students).Object);
        _dbMock.Setup(db => db.TeacherAssignments).Returns(DbSetMockHelper.CreateMockDbSet(assignments).Object);

        var result = await _useCase.ExecuteAsync(student.Id);

        Assert.NotNull(result);
        Assert.Equal(student.Id, result.StudentId);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowConflictWhenAlreadyAssigned()
    {
        var student = User.CreateWithPassword("student@test.com", "hash", "Student");
        var teacherId = _currentUserMock.Object.UserId;
        var existing = TeacherAssignment.Create(student.Id, teacherId, teacherId);
        var students = new List<User> { student };
        var assignments = new List<TeacherAssignment> { existing };

        _dbMock.Setup(db => db.Users).Returns(DbSetMockHelper.CreateMockDbSet(students).Object);
        _dbMock.Setup(db => db.TeacherAssignments).Returns(DbSetMockHelper.CreateMockDbSet(assignments).Object);

        await Assert.ThrowsAsync<ConflictException>(() => _useCase.ExecuteAsync(student.Id));
    }
}
