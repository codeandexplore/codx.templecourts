using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class AddLessonNodeUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly AddLessonNodeUseCase _useCase;

    public AddLessonNodeUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new AddLessonNodeUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_TopLevel_ShouldCreateNodeAtDepth1()
    {
        var version = LessonVersion.Create(Guid.NewGuid(), 1);
        var versionId = version.Id;

        _dbMock.Setup(db => db.LessonVersions.FindAsync(new object[] { versionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version);

        var nodes = new List<LessonNode>().AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var request = new CreateLessonNodeRequest(null, "Section", "Description");
        var result = await _useCase.ExecuteAsync(versionId, request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Depth);
        Assert.Null(result.ParentNodeId);
        Assert.Equal("Section", result.Title);
    }

    [Fact]
    public async Task ExecuteAsync_ParentAtDepth3_ShouldThrowConflict()
    {
        var version = LessonVersion.Create(Guid.NewGuid(), 1);
        var parent = LessonNode.Create(version.Id, null, 3, 0, "Deep", "Desc");
        var versionId = version.Id;

        _dbMock.Setup(db => db.LessonVersions.FindAsync(new object[] { versionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version);

        var nodes = new List<LessonNode> { parent }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var request = new CreateLessonNodeRequest(parent.Key, "Too Deep", "Desc");

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(versionId, request));
    }

    [Fact]
    public async Task ExecuteAsync_ParentHasQuestions_ShouldThrowConflict()
    {
        var version = LessonVersion.Create(Guid.NewGuid(), 1);
        var parent = LessonNode.Create(version.Id, null, 1, 0, "Parent", "Desc");
        var question = Question.Create(parent.Id, 0, Codx.Temple.Domain.Enums.QuestionType.Essay, "Q?");
        parent.Questions.Add(question);
        var versionId = version.Id;

        _dbMock.Setup(db => db.LessonVersions.FindAsync(new object[] { versionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version);

        var nodes = new List<LessonNode> { parent }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var request = new CreateLessonNodeRequest(parent.Key, "Child", "Desc");

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(versionId, request));
    }
}
