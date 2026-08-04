namespace Codx.Temple.Application.DTOs.Auth;

public record UserInfo(Guid Id, string Email, string DisplayName, List<string> Roles);

public record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, UserInfo User);
