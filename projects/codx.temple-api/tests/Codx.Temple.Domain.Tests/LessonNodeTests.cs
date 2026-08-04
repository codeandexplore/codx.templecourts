using Codx.Temple.Domain.Entities;

namespace Codx.Temple.Domain.Tests;

public class LessonNodeTests
{
    [Fact]
    public void Create_Should_SetPropertiesCorrectly()
    {
        var versionId = Guid.NewGuid();
        var node = LessonNode.Create(versionId, null, 1, 0, "Section 1", "Covers John 3:16");

        Assert.NotEqual(Guid.Empty, node.Id);
        Assert.NotEqual(Guid.Empty, node.Key);
        Assert.Equal(versionId, node.LessonVersionId);
        Assert.Null(node.ParentNodeId);
        Assert.Equal(1, node.Depth);
        Assert.Equal(0, node.Order);
        Assert.Equal("Section 1", node.Title);
        Assert.Equal("Covers John 3:16", node.Description);
        Assert.False(node.RequiresPriorSiblingAnswered);
    }

    [Fact]
    public void Create_WithParent_Should_SetDepth2()
    {
        var parentId = Guid.NewGuid();
        var node = LessonNode.Create(Guid.NewGuid(), parentId, 2, 1, "Sub-section", "Description");

        Assert.Equal(parentId, node.ParentNodeId);
        Assert.Equal(2, node.Depth);
    }

    [Fact]
    public void Create_WithGating_Should_SetFlag()
    {
        var node = LessonNode.Create(Guid.NewGuid(), null, 1, 0, "Section", "Desc", requiresPriorSiblingAnswered: true);

        Assert.True(node.RequiresPriorSiblingAnswered);
    }

    [Fact]
    public void Create_MaxDepth3_Should_BeValid()
    {
        var node = LessonNode.Create(Guid.NewGuid(), Guid.NewGuid(), 3, 0, "Deep", "Description");

        Assert.Equal(3, node.Depth);
    }
}
