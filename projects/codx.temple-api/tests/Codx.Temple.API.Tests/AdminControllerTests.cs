using Codx.Temple.API.Controllers;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Codx.Temple.API.Tests;

public class AdminControllerTests
{
    private readonly Mock<IAppDbContext> _dbMock = new();
    private readonly Mock<ICurrentUserAccessor> _currentUserMock = new();

    private readonly Mock<ListRoleAssignmentsUseCase> _listUseCase;
    private readonly Mock<AssignRoleUseCase> _assignUseCase;

    public AdminControllerTests()
    {
        _listUseCase = new Mock<ListRoleAssignmentsUseCase>(_dbMock.Object);
        _assignUseCase = new Mock<AssignRoleUseCase>(_dbMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task ListRoleAssignments_ShouldReturnOk_WithAssignmentList()
    {
        var expected = new List<RoleAssignmentDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "user@example.com", "User", "Teacher", Guid.NewGuid(), DateTimeOffset.UtcNow)
        };

        _listUseCase.Setup(u => u.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new AdminController();
        var result = await controller.ListRoleAssignments(_listUseCase.Object, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AssignRole_ShouldReturnCreatedAtAction_WhenSuccessful()
    {
        var request = new AssignRoleRequest(Guid.NewGuid(), "Teacher");
        var expected = new RoleAssignmentDto(
            Guid.NewGuid(), request.UserId, "test@example.com", "TestUser", "Teacher", Guid.NewGuid(), DateTimeOffset.UtcNow);

        _assignUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new AdminController();
        var result = await controller.AssignRole(request, _assignUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result.Result!;
        createdResult.ActionName.Should().Be("ListRoleAssignments");
        createdResult.Value.Should().BeEquivalentTo(expected);
    }
}
