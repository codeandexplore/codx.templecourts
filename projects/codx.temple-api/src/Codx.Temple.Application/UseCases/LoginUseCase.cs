using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class LoginUseCase
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(IAppDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public virtual async Task<AuthResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || user.PasswordHash is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

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
