using Codx.Temple.API.Controllers;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Codx.Temple.API.Tests;

public class LessonAuthoringControllerTests
{
    private readonly Mock<IAppDbContext> _dbMock = new();
    private readonly Mock<GetLessonTreeUseCase> _getTreeUseCase;
    private readonly Mock<CreateLessonUseCase> _createLessonUseCase;
    private readonly Mock<ListLessonsUseCase> _listLessonsUseCase;
    private readonly Mock<UpdateLessonUseCase> _updateLessonUseCase;
    private readonly Mock<CreateDraftVersionUseCase> _createDraftUseCase;
    private readonly Mock<PublishVersionUseCase> _publishVersionUseCase;
    private readonly Mock<AddLessonNodeUseCase> _addNodeUseCase;
    private readonly Mock<AddQuestionUseCase> _addQuestionUseCase;

    public LessonAuthoringControllerTests()
    {
        _getTreeUseCase = new Mock<GetLessonTreeUseCase>(_dbMock.Object);
        _createLessonUseCase = new Mock<CreateLessonUseCase>(_dbMock.Object);
        _listLessonsUseCase = new Mock<ListLessonsUseCase>(_dbMock.Object);
        _updateLessonUseCase = new Mock<UpdateLessonUseCase>(_dbMock.Object);
        _createDraftUseCase = new Mock<CreateDraftVersionUseCase>(_dbMock.Object);
        _publishVersionUseCase = new Mock<PublishVersionUseCase>(_dbMock.Object);
        _addNodeUseCase = new Mock<AddLessonNodeUseCase>(_dbMock.Object);
        _addQuestionUseCase = new Mock<AddQuestionUseCase>(_dbMock.Object);
    }

    [Fact]
    public async Task CreateLesson_ShouldReturnCreated()
    {
        var request = new CreateLessonRequest(1, "Test Lesson");
        var lessonDto = new LessonDto(Guid.NewGuid(), Guid.NewGuid(), 1, "Test Lesson", "Active", null);
        _createLessonUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(lessonDto);

        var controller = new LessonsController();
        var result = await controller.Create(request, _createLessonUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task ListLessons_ShouldReturnOk()
    {
        var expected = new List<LessonDto>();
        _listLessonsUseCase.Setup(u => u.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new LessonsController();
        var result = await controller.List(_listLessonsUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLessonTree_ShouldReturnOk()
    {
        var versionDto = new LessonVersionDto(Guid.NewGuid(), Guid.NewGuid(), 1, "Published", null, null, DateTimeOffset.UtcNow, []);
        _getTreeUseCase.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(versionDto);

        var controller = new LessonsController();
        var result = await controller.Get(Guid.NewGuid(), _getTreeUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateDraft_ShouldReturnCreated()
    {
        var versionDto = new LessonVersionDto(Guid.NewGuid(), Guid.NewGuid(), 2, "Draft", "Clone", null, DateTimeOffset.UtcNow, []);
        _createDraftUseCase.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CreateLessonVersionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(versionDto);

        var controller = new LessonVersionsController();
        var result = await controller.CreateDraft(Guid.NewGuid(), new CreateLessonVersionRequest(), _createDraftUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Publish_ShouldReturnNoContent()
    {
        var controller = new LessonVersionsController();
        var result = await controller.Publish(Guid.NewGuid(), Guid.NewGuid(), _publishVersionUseCase.Object, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AddLessonNode_ShouldReturnCreated()
    {
        var nodeDto = new LessonNodeDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, 1, 0, "Section", "Desc", false, [], []);
        _addNodeUseCase.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CreateLessonNodeRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(nodeDto);

        var controller = new LessonNodesController();
        var result = await controller.Add(Guid.NewGuid(), new CreateLessonNodeRequest(null, "Section", "Desc"), _addNodeUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task AddQuestion_ShouldReturnCreated()
    {
        var questionDto = new QuestionDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, "Essay", "Prompt?", null, null);
        _addQuestionUseCase.Setup(u => u.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CreateQuestionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(questionDto);

        var controller = new QuestionsController();
        var result = await controller.Add(Guid.NewGuid(), new CreateQuestionRequest("Essay", "Prompt?"), _addQuestionUseCase.Object, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }
}
