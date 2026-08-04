using System.Security.Claims;

namespace Codx.Temple.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string displayName, IEnumerable<string> roles);
    (string token, string hash, DateTimeOffset expiresAt) GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
