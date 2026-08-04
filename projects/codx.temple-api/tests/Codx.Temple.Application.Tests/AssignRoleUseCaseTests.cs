using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Codx.Temple.Application.Tests;

public class AssignRoleUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<ICurrentUserAccessor> _currentUserMock;
    private readonly AssignRoleUseCase _useCase;

    public AssignRoleUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _currentUserMock = new Mock<ICurrentUserAccessor>();
        _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
        _useCase = new AssignRoleUseCase(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidRole_ShouldThrowConflict()
    {
        var request = new AssignRoleRequest(Guid.NewGuid(), "InvalidRole");
        var roleAssignments = new List<RoleAssignment>().AsQueryable();
        var roleDbSet = DbSetMockHelper.CreateMockDbSet(roleAssignments);
        _dbMock.Setup(db => db.RoleAssignments).Returns(roleDbSet.Object);

        var users = new List<User>().AsQueryable();
        var userDbSet = DbSetMockHelper.CreateMockDbSet(users);
        _dbMock.Setup(db => db.Users).Returns(userDbSet.Object);

        await Assert.ThrowsAsync<ConflictException>(() => _useCase.ExecuteAsync(request));
    }
}
