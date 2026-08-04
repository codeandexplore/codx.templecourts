using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Codx.Temple.API.IntegrationTests;

public static class JwtHelper
{
    private const string Secret = "test-secret-key-thats-long-enough-for-hs256-minimum-32-bytes!!";
    private const string Issuer = "templecourts-test";
    private const string Audience = "templecourts-test";

    public static string GenerateAccessToken(Guid userId, string email, string displayName, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("display_name", displayName),
        };

        var roleClaim = string.Join(",", roles);
        if (!string.IsNullOrEmpty(roleClaim))
            claims.Add(new Claim("application_role", roleClaim));

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
