namespace Codx.Temple.Application.Abstractions;

public record GoogleUserInfo(string Email, string Name, string GoogleId);

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
