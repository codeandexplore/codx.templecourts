using System.Net;
using System.Net.Http.Json;
using Codx.Temple.Application.DTOs.Lessons;
using FluentAssertions;

namespace Codx.Temple.API.IntegrationTests;

[Collection("IntegrationTests")]
public class ClonePreservesKeysIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public ClonePreservesKeysIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeepClone_ShouldPreserveKeys_AndCreateNewIds()
    {
        var adminClient = _factory.CreateAdminClient();

        var createResp = await adminClient.PostAsJsonAsync("/api/lessons", new { number = 500, title = "Clone Key Test" });
        var lesson = await createResp.Content.ReadFromJsonAsync<LessonDto>();

        var draftResp = await adminClient.PostAsJsonAsync($"/api/lessons/{lesson!.Key}/versions", new { });
        var draft = await draftResp.Content.ReadFromJsonAsync<LessonVersionDto>();

        var nodeResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft!.Id}/nodes",
            new { parentNodeKey = (Guid?)null, title = "Root", description = "Top level" });
        var node1 = await nodeResp.Content.ReadFromJsonAsync<LessonNodeDto>();

        var childResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft.Id}/nodes",
            new { parentNodeKey = node1!.Key, title = "Child", description = "Child node" });
        var node2 = await childResp.Content.ReadFromJsonAsync<LessonNodeDto>();

        var qResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-nodes/{node2!.Key}/questions",
            new { questionType = "Essay", promptText = "Test question?" });
        var question = await qResp.Content.ReadFromJsonAsync<QuestionDto>();

        await adminClient.PostAsync($"/api/lessons/{lesson.Key}/versions/{draft.Id}/publish", null);

        var cloneResp = await adminClient.PostAsJsonAsync(
            $"/api/lessons/{lesson.Key}/versions", new { changeNotes = "Cloned draft" });
        var clone = await cloneResp.Content.ReadFromJsonAsync<LessonVersionDto>();

        clone.Should().NotBeNull();
        clone!.Nodes.Should().HaveCount(1);
        var clonedRoot = clone.Nodes[0];
        clonedRoot.Key.Should().Be(node1.Key);
        clonedRoot.Id.Should().NotBe(node1.Id);
        clonedRoot.Title.Should().Be("Root");

        clonedRoot.Children.Should().HaveCount(1);
        var clonedChild = clonedRoot.Children[0];
        clonedChild.Key.Should().Be(node2.Key);
        clonedChild.Id.Should().NotBe(node2.Id);

        clonedChild.Questions.Should().HaveCount(1);
        var clonedQuestion = clonedChild.Questions[0];
        clonedQuestion.Key.Should().Be(question!.Key);
        clonedQuestion.Id.Should().NotBe(question.Id);
        clonedQuestion.PromptText.Should().Be("Test question?");
    }
}
