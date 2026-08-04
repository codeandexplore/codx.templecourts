using Codx.Temple.Application.Abstractions;

namespace Codx.Temple.API.IntegrationTests;

public class TestGoogleAuthService : IGoogleAuthService
{
    private readonly string _validToken;
    private readonly string _email;
    private readonly string _name;
    private readonly string _googleId;

    public TestGoogleAuthService(string validToken = "valid-test-google-token",
        string email = "google-user@example.com",
        string name = "Google User",
        string googleId = "g-test-123")
    {
        _validToken = validToken;
        _email = email;
        _name = name;
        _googleId = googleId;
    }

    public Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (idToken == _validToken)
            return Task.FromResult(new GoogleUserInfo(_email, _name, _googleId));

        throw new Application.Exceptions.UnauthorizedException("Invalid Google token.");
    }
}
