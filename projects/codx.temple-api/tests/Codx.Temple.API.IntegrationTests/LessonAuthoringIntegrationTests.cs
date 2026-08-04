using System.Net;
using System.Net.Http.Json;
using Codx.Temple.Application.DTOs.Lessons;
using FluentAssertions;

namespace Codx.Temple.API.IntegrationTests;

[Collection("IntegrationTests")]
public class LessonAuthoringIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public LessonAuthoringIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullAuthoringWorkflow_CreateLesson_AddNodes_Publish_ReadAsStudent()
    {
        var adminClient = _factory.CreateAdminClient();

        var createReq = new { number = 100, title = "Integration Test Lesson" };
        var createResp = await adminClient.PostAsJsonAsync("/api/lessons", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var lesson = await createResp.Content.ReadFromJsonAsync<LessonDto>();
        lesson.Should().NotBeNull();

        var draftResp = await adminClient.PostAsJsonAsync(
            $"/api/lessons/{lesson!.Key}/versions", new { });
        draftResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var draft = await draftResp.Content.ReadFromJsonAsync<LessonVersionDto>();
        draft.Should().NotBeNull();

        var nodeResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft!.Id}/nodes",
            new { parentNodeKey = (Guid?)null, title = "Section 1", description = "Test section" });
        nodeResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var node = await nodeResp.Content.ReadFromJsonAsync<LessonNodeDto>();

        var qResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-nodes/{node!.Key}/questions",
            new { questionType = "Essay", promptText = "What do you think?" });
        qResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var pubResp = await adminClient.PostAsync(
            $"/api/lessons/{lesson.Key}/versions/{draft.Id}/publish", null);
        pubResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var studentClient = _factory.CreateStudentClient();
        var readResp = await studentClient.GetAsync($"/api/lessons/{lesson.Key}");
        readResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var tree = await readResp.Content.ReadFromJsonAsync<LessonVersionDto>();
        tree.Should().NotBeNull();
        tree!.Nodes.Should().HaveCount(1);
    }

    [Fact]
    public async Task StructuralRules_MaxDepth3_Rejected()
    {
        var adminClient = _factory.CreateAdminClient();

        var createReq = new { number = 200, title = "Depth Test" };
        var createResp = await adminClient.PostAsJsonAsync("/api/lessons", createReq);
        var lesson = await createResp.Content.ReadFromJsonAsync<LessonDto>();

        var draftResp = await adminClient.PostAsJsonAsync($"/api/lessons/{lesson!.Key}/versions", new { });
        var draft = await draftResp.Content.ReadFromJsonAsync<LessonVersionDto>();

        var n1 = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft!.Id}/nodes",
            new { parentNodeKey = (Guid?)null, title = "Depth 1", description = "Top" });
        var node1 = await n1.Content.ReadFromJsonAsync<LessonNodeDto>();

        var n2 = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft.Id}/nodes",
            new { parentNodeKey = node1!.Key, title = "Depth 2", description = "Mid" });
        var node2 = await n2.Content.ReadFromJsonAsync<LessonNodeDto>();

        var n3 = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft.Id}/nodes",
            new { parentNodeKey = node2!.Key, title = "Depth 3", description = "Bottom" });
        n3.StatusCode.Should().Be(HttpStatusCode.Created);
        var node3 = await n3.Content.ReadFromJsonAsync<LessonNodeDto>();

        var n4 = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft.Id}/nodes",
            new { parentNodeKey = node3!.Key, title = "Depth 4 Attempt", description = "Should fail" });
        n4.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task LeafOnlyRule_QuestionOnNonLeaf_Rejected()
    {
        var adminClient = _factory.CreateAdminClient();

        var createResp = await adminClient.PostAsJsonAsync("/api/lessons", new { number = 300, title = "Leaf Test" });
        var lesson = await createResp.Content.ReadFromJsonAsync<LessonDto>();
        var draftResp = await adminClient.PostAsJsonAsync($"/api/lessons/{lesson!.Key}/versions", new { });
        var draft = await draftResp.Content.ReadFromJsonAsync<LessonVersionDto>();

        var nodeResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft!.Id}/nodes",
            new { parentNodeKey = (Guid?)null, title = "Parent", description = "With child" });
        var node = await nodeResp.Content.ReadFromJsonAsync<LessonNodeDto>();

        await adminClient.PostAsJsonAsync(
            $"/api/lesson-versions/{draft.Id}/nodes",
            new { parentNodeKey = node!.Key, title = "Child", description = "Makes parent non-leaf" });

        var qResp = await adminClient.PostAsJsonAsync(
            $"/api/lesson-nodes/{node.Key}/questions",
            new { questionType = "Essay", promptText = "Should fail?" });
        qResp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PublishValidation_NoTopLevelNodes_Rejected()
    {
        var adminClient = _factory.CreateAdminClient();

        var createResp = await adminClient.PostAsJsonAsync("/api/lessons", new { number = 400, title = "Publish Test" });
        var lesson = await createResp.Content.ReadFromJsonAsync<LessonDto>();
        var draftResp = await adminClient.PostAsJsonAsync($"/api/lessons/{lesson!.Key}/versions", new { });
        var draft = await draftResp.Content.ReadFromJsonAsync<LessonVersionDto>();

        var pubResp = await adminClient.PostAsync(
            $"/api/lessons/{lesson.Key}/versions/{draft!.Id}/publish", null);
        pubResp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
