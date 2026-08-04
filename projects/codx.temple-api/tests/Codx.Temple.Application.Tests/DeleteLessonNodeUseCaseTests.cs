using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class DeleteLessonNodeUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly DeleteLessonNodeUseCase _useCase;

    public DeleteLessonNodeUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _useCase = new DeleteLessonNodeUseCase(_dbMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingNode_ShouldRemoveIt()
    {
        var version = LessonVersion.Create(Guid.NewGuid(), 1);
        var node = LessonNode.Create(version.Id, null, 1, 0, "Section", "Desc");
        var nodeKey = node.Key;
        var versionId = version.Id;

        var nodes = new List<LessonNode> { node }.AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);
        _dbMock.Setup(db => db.LessonVersions.FindAsync(new object[] { versionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(version);

        await _useCase.ExecuteAsync(versionId, nodeKey);

        _dbMock.Verify(db => db.LessonNodes.Remove(node), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownNode_ShouldThrowNotFound()
    {
        var versionId = Guid.NewGuid();
        var nodes = new List<LessonNode>().AsQueryable();
        _dbMock.Setup(db => db.LessonNodes).Returns(DbSetMockHelper.CreateMockDbSet(nodes).Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(versionId, Guid.NewGuid()));
    }
}
