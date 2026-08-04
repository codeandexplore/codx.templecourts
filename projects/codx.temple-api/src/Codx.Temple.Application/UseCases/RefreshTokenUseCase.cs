using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class RefreshTokenUseCase
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(IAppDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public virtual async Task<AuthResponse> ExecuteAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _db.Users
            .Include(u => u.RoleAssignments)
            .Where(u => u.RefreshTokenHash != null && u.RefreshTokenExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

        Domain.Entities.User? matchingUser = null;
        foreach (var user in users)
        {
            if (user.RefreshTokenHash is not null && _passwordHasher.Verify(request.RefreshToken, user.RefreshTokenHash))
            {
                matchingUser = user;
                break;
            }
        }

        if (matchingUser is null)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var roles = matchingUser.RoleAssignments.Select(r => r.Role.ToString()).ToList();
        var (newRefreshToken, newRefreshTokenHash, refreshExpires) = _tokenService.GenerateRefreshToken();
        matchingUser.SetRefreshToken(newRefreshTokenHash, refreshExpires);

        await _db.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(matchingUser.Id, matchingUser.Email, matchingUser.DisplayName, roles);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            expiresAt,
            new UserInfo(matchingUser.Id, matchingUser.Email, matchingUser.DisplayName, roles));
    }
}
