using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class GoogleAuthUseCase
{
    private readonly IAppDbContext _db;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITokenService _tokenService;

    public GoogleAuthUseCase(IAppDbContext db, IGoogleAuthService googleAuthService, ITokenService tokenService)
    {
        _db = db;
        _googleAuthService = googleAuthService;
        _tokenService = tokenService;
    }

    public virtual async Task<AuthResponse> ExecuteAsync(GoogleAuthRequest request, CancellationToken cancellationToken = default)
    {
        var googleUser = await _googleAuthService.ValidateIdTokenAsync(request.IdToken, cancellationToken);

        var email = googleUser.Email.ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is not null)
        {
            if (user.GoogleId is null)
                user.LinkGoogleAccount(googleUser.GoogleId);
        }
        else
        {
            user = User.CreateWithGoogle(googleUser.Email, googleUser.GoogleId, googleUser.Name);
            _db.Users.Add(user);
        }

        var roles = user.RoleAssignments.Select(r => r.Role.ToString()).ToList();
        var (refreshToken, refreshTokenHash, refreshExpires) = _tokenService.GenerateRefreshToken();
        user.SetRefreshToken(refreshTokenHash, refreshExpires);

        await _db.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.DisplayName, roles);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        return new AuthResponse(
            accessToken,
            refreshToken,
            expiresAt,
            new UserInfo(user.Id, user.Email, user.DisplayName, roles));
    }
}
