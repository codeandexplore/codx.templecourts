namespace Codx.Temple.Application.DTOs.Auth;

public sealed class CallerContextDto
{
    public string UserId { get; init; } = string.Empty;
    public string? ClientId { get; init; }
    public string? Email { get; init; }
    public string? Role { get; init; }
}
