using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Moq;

namespace Codx.Temple.Application.Tests;

public class PublishVersionUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly PublishVersionUseCase _useCase;

    public PublishVersionUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new PublishVersionUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDraft_ShouldPublish()
    {
        var lesson = Lesson.Create(1, "Lesson 1");
        var version = LessonVersion.Create(lesson.Id, 1);
        version.Publish();
        lesson.SetCurrentPublishedVersion(version.Id);

        var draft = LessonVersion.Create(lesson.Id, 2, "Changes", version.Id);
        var node = LessonNode.Create(draft.Id, null, 1, 0, "Top", "Desc");
        draft.Nodes.Add(node);

        var lessons = new List<Lesson> { lesson }.AsQueryable();
        var versions = new List<LessonVersion> { version, draft }.AsQueryable();

        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonVersions).Returns(DbSetMockHelper.CreateMockDbSet(versions).Object);

        await _useCase.ExecuteAsync(lesson.Key, draft.Id);

        Assert.Equal(LessonVersionStatus.Published, draft.Status);
        Assert.NotNull(draft.PublishedAt);
        Assert.Equal(LessonVersionStatus.Retired, version.Status);
        Assert.Equal(draft.Id, lesson.CurrentPublishedVersionId);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoTopLevelNodes_ShouldThrowConflict()
    {
        var lesson = Lesson.Create(1, "Lesson 1");
        var draft = LessonVersion.Create(lesson.Id, 1);

        var lessons = new List<Lesson> { lesson }.AsQueryable();
        var versions = new List<LessonVersion> { draft }.AsQueryable();

        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonVersions).Returns(DbSetMockHelper.CreateMockDbSet(versions).Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(lesson.Key, draft.Id));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonDraftVersion_ShouldThrowConflict()
    {
        var lesson = Lesson.Create(1, "Lesson 1");
        var version = LessonVersion.Create(lesson.Id, 1);
        version.Publish();
        var node = LessonNode.Create(version.Id, null, 1, 0, "Top", "Desc");
        version.Nodes.Add(node);

        var lessons = new List<Lesson> { lesson }.AsQueryable();
        var versions = new List<LessonVersion> { version }.AsQueryable();

        _dbMock.Setup(db => db.Lessons).Returns(DbSetMockHelper.CreateMockDbSet(lessons).Object);
        _dbMock.Setup(db => db.LessonVersions).Returns(DbSetMockHelper.CreateMockDbSet(versions).Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(lesson.Key, version.Id));
    }
}
