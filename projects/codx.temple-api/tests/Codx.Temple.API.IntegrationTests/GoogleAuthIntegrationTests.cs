using System.Net;
using System.Net.Http.Json;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Codx.Temple.API.IntegrationTests;

[Collection("IntegrationTests")]
public class GoogleAuthIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client;

    public GoogleAuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        var factoryWithOverride = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IGoogleAuthService));
                if (descriptor != null) services.Remove(descriptor);

                services.AddScoped<IGoogleAuthService>(_ => new TestGoogleAuthService());
            });
        });

        _client = factoryWithOverride.CreateClient();
    }

    [Fact]
    public async Task GoogleAuth_NewUser_ShouldCreateAndReturnTokens()
    {
        var request = new GoogleAuthRequest("valid-test-google-token");
        var response = await _client.PostAsJsonAsync("/auth/google", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("google-user@example.com");
        result.User.DisplayName.Should().Be("Google User");
    }

    [Fact]
    public async Task GoogleAuth_ReturningUser_ShouldLoginWithExistingAccount()
    {
        var request = new GoogleAuthRequest("valid-test-google-token");

        await _client.PostAsJsonAsync("/auth/google", request);
        var response = await _client.PostAsJsonAsync("/auth/google", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.User.Email.Should().Be("google-user@example.com");
    }

    [Fact]
    public async Task GoogleAuth_InvalidToken_ShouldReturnUnauthorized()
    {
        var request = new GoogleAuthRequest("invalid-token");
        var response = await _client.PostAsJsonAsync("/auth/google", request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
