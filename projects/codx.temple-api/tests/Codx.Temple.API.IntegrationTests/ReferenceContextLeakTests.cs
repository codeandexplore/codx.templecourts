using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Codx.Temple.API.IntegrationTests;

[Collection("IntegrationTests")]
public class ReferenceContextLeakTests
{
    private readonly CustomWebApplicationFactory _factory;

    public ReferenceContextLeakTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateTestClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Controllers.TestQuestionsController).Assembly));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Student_Role_ShouldNotSee_ReferenceContext()
    {
        var client = CreateTestClient();
        var token = JwtHelper.GenerateAccessToken(Guid.NewGuid(), "student@test.com", "Student", new[] { "Student" });
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/test/question");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        doc.RootElement.TryGetProperty("referenceContext", out _).Should().BeFalse();
    }

    [Fact]
    public async Task Admin_Role_ShouldSee_ReferenceContext()
    {
        var client = CreateTestClient();
        var token = JwtHelper.GenerateAccessToken(Guid.NewGuid(), "admin@test.com", "Admin", new[] { "Admin", "Teacher" });
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/test/question");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        doc.RootElement.TryGetProperty("referenceContext", out var refCtx).Should().BeTrue();
        refCtx.GetProperty("answer").GetString().Should().Be("42");
    }

    [Fact]
    public async Task Unauthenticated_ShouldStillSee_ReferenceContext()
    {
        var client = CreateTestClient();

        // Unauthenticated requests still get reference_context
        // because the serialization only strips for Student role
        var response = await client.GetAsync("/test/question");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        doc.RootElement.TryGetProperty("referenceContext", out _).Should().BeTrue();
    }
}
