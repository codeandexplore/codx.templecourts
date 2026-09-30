using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Application.Tests;

public class TreeTraversalTests
{
    [Fact]
    public void GetTreeTraversalQuestionKeys_Depth3_MixedBranches_ReturnsDepthFirstLeafOrder()
    {
        var versionId = Guid.NewGuid();

        // Root A (depth 1)
        var rootA = LessonNode.Create(versionId, null, 1, 0, "Root A", "");
        // Root A children (depth 2)
        var a1 = LessonNode.Create(versionId, rootA.Id, 2, 0, "A1", "");
        var a2 = LessonNode.Create(versionId, rootA.Id, 2, 1, "A2", "");
        // A1 children (depth 3, leaves)
        var a1a = LessonNode.Create(versionId, a1.Id, 3, 0, "A1a", "");
        var a1b = LessonNode.Create(versionId, a1.Id, 3, 1, "A1b", "");
        // Root B (depth 1, leaf)
        var rootB = LessonNode.Create(versionId, null, 1, 1, "Root B", "");

        // Questions only on leaves
        var q1 = Question.Create(a1a.Id, 0, QuestionType.Essay, "Q1");
        var q2 = Question.Create(a1b.Id, 0, QuestionType.Essay, "Q2");
        var q3 = Question.Create(a2.Id, 0, QuestionType.Essay, "Q3");
        var q4 = Question.Create(rootB.Id, 0, QuestionType.Essay, "Q4");

        var nodes = new List<LessonNode> { rootA, a1, a2, a1a, a1b, rootB };
        var questions = new List<Question> { q1, q2, q3, q4 };

        var ordered = GetSessionQuestionsUseCase.GetTreeTraversalQuestionKeys(nodes, questions);

        Assert.Equal(new[] { q1.Key, q2.Key, q3.Key, q4.Key }, ordered);
    }
}
