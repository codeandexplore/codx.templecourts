using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class AddQuestionUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly AddQuestionUseCase _useCase;

    public AddQuestionUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new AddQuestionUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_OnLeafNode_ShouldCreateQuestion()
    {
        var node = LessonNode.Create(Guid.NewGuid(), null, 1, 0, "Section", "Desc");
        var nodeKey = node.Key;

        var nodes = new List<LessonNode> { node }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var questions = new List<Question>().AsQueryable();
        _dbMock.Setup(db => db.Questions).Returns(DbSetMockHelper.CreateMockDbSet(questions).Object);

        var request = new CreateQuestionRequest("Essay", "What do you think?");
        var result = await _useCase.ExecuteAsync(nodeKey, request);

        Assert.NotNull(result);
        Assert.Equal("Essay", result.QuestionType);
        Assert.Equal("What do you think?", result.PromptText);
        Assert.Equal(0, result.Order);
    }

    [Fact]
    public async Task ExecuteAsync_OnNodeWithChildren_ShouldThrowConflict()
    {
        var parent = LessonNode.Create(Guid.NewGuid(), null, 1, 0, "Parent", "Desc");
        var child = LessonNode.Create(parent.LessonVersionId, parent.Id, 2, 0, "Child", "Desc");
        parent.ChildNodes.Add(child);

        var nodes = new List<LessonNode> { parent }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var request = new CreateQuestionRequest("Essay", "Prompt?");

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(parent.Key, request));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidQuestionType_ShouldThrowConflict()
    {
        var node = LessonNode.Create(Guid.NewGuid(), null, 1, 0, "Section", "Desc");

        var nodes = new List<LessonNode> { node }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        var request = new CreateQuestionRequest("InvalidType", "Prompt?");

        await Assert.ThrowsAsync<ConflictException>(() =>
            _useCase.ExecuteAsync(node.Key, request));
    }
}
