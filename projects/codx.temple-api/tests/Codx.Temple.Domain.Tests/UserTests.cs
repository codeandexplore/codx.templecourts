using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class UserTests
{
    [Fact]
    public void CreateWithPassword_Should_SetPropertiesCorrectly()
    {
        var user = User.CreateWithPassword("test@example.com", "hashed-password", "Test User");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Null(user.GoogleId);
        Assert.Equal("Test User", user.DisplayName);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.True(user.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void CreateWithGoogle_Should_SetPropertiesCorrectly()
    {
        var user = User.CreateWithGoogle("google@example.com", "google-id-123", "Google User");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("google@example.com", user.Email);
        Assert.Null(user.PasswordHash);
        Assert.Equal("google-id-123", user.GoogleId);
        Assert.Equal("Google User", user.DisplayName);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void LinkGoogleAccount_Should_SetGoogleId()
    {
        var user = User.CreateWithPassword("test@example.com", "hash", "Test");
        Assert.Null(user.GoogleId);

        user.LinkGoogleAccount("new-google-id");
        Assert.Equal("new-google-id", user.GoogleId);
    }

    [Fact]
    public void SetRefreshToken_Should_SetTokenHashAndExpiry()
    {
        var user = User.CreateWithPassword("test@example.com", "hash", "Test");
        var expires = DateTimeOffset.UtcNow.AddDays(7);

        user.SetRefreshToken("refresh-hash", expires);

        Assert.Equal("refresh-hash", user.RefreshTokenHash);
        Assert.Equal(expires, user.RefreshTokenExpiresAt);
    }

    [Fact]
    public void ClearRefreshToken_Should_NullifyTokenFields()
    {
        var user = User.CreateWithPassword("test@example.com", "hash", "Test");
        user.SetRefreshToken("refresh-hash", DateTimeOffset.UtcNow.AddDays(7));

        user.ClearRefreshToken();

        Assert.Null(user.RefreshTokenHash);
        Assert.Null(user.RefreshTokenExpiresAt);
    }
}
