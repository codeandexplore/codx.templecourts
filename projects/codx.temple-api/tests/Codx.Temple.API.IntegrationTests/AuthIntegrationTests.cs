using System.Net;
using System.Net.Http.Json;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.DTOs.Users;
using FluentAssertions;

namespace Codx.Temple.API.IntegrationTests;

[Collection("IntegrationTests")]
public class AuthIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullAuthFlow_Register_Login_Me_ShouldSucceed()
    {
        var client = _factory.CreateClient();

        var registerRequest = new { email = "user@test.com", password = "TestPass123!", displayName = "Test User" };
        var registerResponse = await client.PostAsJsonAsync("/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        authResult.Should().NotBeNull();
        authResult!.AccessToken.Should().NotBeNullOrEmpty();
        authResult.RefreshToken.Should().NotBeNullOrEmpty();
        authResult.User.Email.Should().Be("user@test.com");
        authResult.User.Roles.Should().Contain("Student");

        var loginRequest = new { email = "user@test.com", password = "TestPass123!" };
        var loginResponse = await client.PostAsJsonAsync("/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loginResult.Should().NotBeNull();
        loginResult!.AccessToken.Should().NotBeNullOrEmpty();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var meResponse = await client.GetAsync("/auth/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var meResult = await meResponse.Content.ReadFromJsonAsync<UserProfileDto>();
        meResult.Should().NotBeNull();
        meResult!.Email.Should().Be("user@test.com");
        meResult.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task UnauthorizedAccess_NoToken_ShouldReturn401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/auth/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnauthorizedAccess_StudentAccessingAdminEndpoint_ShouldReturn403()
    {
        var studentClient = _factory.CreateStudentClient();
        var response = await studentClient.GetAsync("/admin/role-assignments");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ShouldReturnNewTokens()
    {
        var client = _factory.CreateClient();

        var registerRequest = new { email = "refresh-test@test.com", password = "TestPass123!", displayName = "Refresh" };
        var registerResponse = await client.PostAsJsonAsync("/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var refreshRequest = new { refreshToken = authResult!.RefreshToken };
        var refreshResponse = await client.PostAsJsonAsync("/auth/refresh", refreshRequest);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        refreshResult.Should().NotBeNull();
        refreshResult!.AccessToken.Should().NotBeNullOrEmpty();
        refreshResult.RefreshToken.Should().NotBe(authResult.RefreshToken);
    }
}
