using Codx.Temple.API.Controllers;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Codx.Temple.API.Tests;

public class TeacherAssignmentsControllerTests
{
    private readonly Mock<IAppDbContext> _dbMock = new();

    [Fact]
    public async Task Claim_ShouldReturnOk()
    {
        var mock = new Mock<ClaimStudentUseCase>(_dbMock.Object, null!);
        var expected = new TeacherAssignmentDto(Guid.NewGuid(), Guid.NewGuid(), "s@t.com", "Student", Guid.NewGuid(), "t@t.com", "Teacher", "Active", DateTimeOffset.UtcNow, null);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new TeacherAssignmentsController();
        var result = await controller.Claim(Guid.NewGuid(), mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Reassign_ShouldReturnOk()
    {
        var mock = new Mock<ReassignStudentUseCase>(_dbMock.Object, null!);
        var expected = new TeacherAssignmentDto(Guid.NewGuid(), Guid.NewGuid(), "s@t.com", "Student", Guid.NewGuid(), "t@t.com", "Teacher", "Active", DateTimeOffset.UtcNow, null);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<ReassignStudentRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new AdminController();
        var result = await controller.Reassign(new ReassignStudentRequest(Guid.NewGuid(), Guid.NewGuid()), mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
