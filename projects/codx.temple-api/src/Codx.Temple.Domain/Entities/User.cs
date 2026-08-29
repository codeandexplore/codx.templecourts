using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public string DisplayName { get; private set; } = null!;
    public string? RefreshTokenHash { get; private set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public ICollection<RoleAssignment> RoleAssignments { get; private set; } = new List<RoleAssignment>();

    private User() { }

    public static User CreateWithPassword(string email, string passwordHash, string displayName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            DisplayName = displayName,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static User CreateWithGoogle(string email, string googleId, string displayName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            GoogleId = googleId,
            DisplayName = displayName,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void LinkGoogleAccount(string googleId)
    {
        GoogleId = googleId;
    }

    public void SetRefreshToken(string refreshTokenHash, DateTimeOffset expiresAt)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }

    public void ResetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        ClearRefreshToken();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
    }
}
