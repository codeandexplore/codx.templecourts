using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Codx.Temple.Infrastructure.Auth;

public sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

    public GoogleAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _clientId = configuration["GoogleAuth:ClientId"]
            ?? throw new InvalidOperationException("GoogleAuth:ClientId is not configured.");
        _httpClientFactory = httpClientFactory;

        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://accounts.google.com/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever(httpClientFactory.CreateClient("GoogleAuth")));
    }

    public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var config = await _configurationManager.GetConfigurationAsync(cancellationToken);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = new[] { "accounts.google.com", "https://accounts.google.com" },
            ValidateAudience = true,
            ValidAudience = _clientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys,
            NameClaimType = ""
        };

        try
        {
            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out _);

            var email = principal.FindFirstValue(ClaimTypes.Email)
                ?? principal.FindFirstValue("email");

            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedException("Google token does not contain an email claim.");

            var name = principal.FindFirstValue("name") ?? "";
            var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub")!;

            return new GoogleUserInfo(email, name, sub);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedException($"Invalid Google token: {ex.Message}");
        }
    }
}
