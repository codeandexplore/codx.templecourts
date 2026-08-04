using Codx.Temple.Application.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace Codx.Temple.API.Middleware;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            return sub is not null && Guid.TryParse(sub, out var guid) ? guid : Guid.Empty;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value;

    public string? DisplayName =>
        _httpContextAccessor.HttpContext?.User.FindFirst("display_name")?.Value;

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst("application_role")?.Value;
            if (string.IsNullOrEmpty(roleClaim))
                return Array.Empty<string>();
            return roleClaim.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public bool IsInRole(string role) => Roles.Contains(role);
}
