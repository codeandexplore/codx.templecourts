using System.Security.Claims;
using Codx.Temple.API.Controllers;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.DTOs.StudentNotes;
using Codx.Temple.Application.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Codx.Temple.API.Tests;

public class StudentControllersTests
{
    private readonly Mock<IAppDbContext> _dbMock = new();

    [Fact]
    public async Task StartAttempt_ShouldReturnOk()
    {
        var mock = new Mock<StartLessonAttemptUseCase>(_dbMock.Object, null!);
        var expected = new LessonAttemptDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "InProgress", DateTimeOffset.UtcNow, null, []);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new StudentAttemptsController();
        var result = await controller.Start(Guid.NewGuid(), mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAttempt_ShouldReturnOk()
    {
        var mock = new Mock<GetAttemptUseCase>(_dbMock.Object, null!);
        var expected = new LessonAttemptDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "InProgress", DateTimeOffset.UtcNow, null, []);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new StudentAttemptsController();
        var result = await controller.Get(Guid.NewGuid(), mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SubmitAnswer_ShouldReturnOk()
    {
        var mock = new Mock<SubmitAnswerUseCase>(_dbMock.Object, null!);
        var expected = new StudentAnswerDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "value", "prompt", "Essay", DateTimeOffset.UtcNow);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<SubmitAnswerRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new StudentAnswersController();
        var request = new SubmitAnswerRequest(Guid.NewGuid(), "test answer");
        var result = await controller.Submit(Guid.NewGuid(), request, mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateNote_ShouldReturnOk()
    {
        var mock = new Mock<CreateStudentQuestionNoteUseCase>(_dbMock.Object, null!);
        var expected = new StudentQuestionNoteDto(Guid.NewGuid(), Guid.NewGuid(), "text", DateTimeOffset.UtcNow);
        mock.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CreateNoteRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new StudentNotesController();
        var result = await controller.Create(Guid.NewGuid(), new CreateNoteRequest("text"), mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteNote_ShouldReturnNoContent()
    {
        var mock = new Mock<DeleteStudentQuestionNoteUseCase>(_dbMock.Object, null!);

        var controller = new StudentNotesController();
        var result = await controller.Delete(Guid.NewGuid(), mock.Object, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ListLessons_ShouldReturnOkForAuthenticated()
    {
        var mock = new Mock<ListStudentLessonsUseCase>(_dbMock.Object);
        var expected = new List<LessonDto> { new(Guid.NewGuid(), Guid.NewGuid(), 1, "Test", "Active", null) };
        mock.Setup(u => u.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new LessonsController();
        var result = await controller.List(mock.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
