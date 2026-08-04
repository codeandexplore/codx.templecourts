using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Moq;

namespace Codx.Temple.Application.Tests;

public class CreateDraftVersionUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly CreateDraftVersionUseCase _useCase;

    public CreateDraftVersionUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new CreateDraftVersionUseCase(_dbMock.Object);
    }

    [Fact(Skip = "Requires Include/ThenInclude support in mock DbSet provider")]
    public async Task ExecuteAsync_WithPublishedVersion_ShouldCloneTree()
    {
        var lesson = Lesson.Create(1, "Lesson 1");
        var version = LessonVersion.Create(lesson.Id, 1);
        version.Publish();
        lesson.SetCurrentPublishedVersion(version.Id);

        var node1 = LessonNode.Create(version.Id, null, 1, 0, "Section", "Desc");
        var q1 = Question.Create(node1.Id, 0, QuestionType.Essay, "Prompt?");
        version.Nodes.Add(node1);
        node1.Questions.Add(q1);

        var lessons = new List<Lesson> { lesson }.AsQueryable();
        var versions = new List<LessonVersion> { version }.AsQueryable();

        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonVersions).Returns(DbSetMockHelper.CreateMockDbSet(versions).Object);

        var result = await _useCase.ExecuteAsync(lesson.Key, new CreateLessonVersionRequest("Draft clone"));

        Assert.NotNull(result);
        Assert.Equal(2, result.VersionNumber);
        Assert.Equal("Draft", result.Status);
        Assert.Equal("Draft clone", result.ChangeNotes);
        Assert.Single(result.Nodes);
        Assert.Single(result.Nodes[0].Questions);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoPublishedVersion_ShouldCreateEmptyDraft()
    {
        var lesson = Lesson.Create(1, "Lesson 1");
        var lessons = new List<Lesson> { lesson }.AsQueryable();
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonVersions).Returns(DbSetMockHelper.CreateMockDbSet(new List<LessonVersion>().AsQueryable()).Object);

        var result = await _useCase.ExecuteAsync(lesson.Key, new CreateLessonVersionRequest());

        Assert.NotNull(result);
        Assert.Equal(1, result.VersionNumber);
        Assert.Equal("Draft", result.Status);
        Assert.Empty(result.Nodes);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownLesson_ShouldThrowNotFound()
    {
        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(new List<Lesson>().AsQueryable()).Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), new CreateLessonVersionRequest()));
    }
}
