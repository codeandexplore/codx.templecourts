using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codx.Temple.Application.UseCases;

public class RegisterUseCase
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUseCase(IAppDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public virtual async Task<AuthResponse> ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
            throw new ConflictException("A user with this email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.CreateWithPassword(request.Email, passwordHash, request.DisplayName);

        var roles = new List<string>();
        var (refreshToken, refreshTokenHash, refreshExpires) = _tokenService.GenerateRefreshToken();
        user.SetRefreshToken(refreshTokenHash, refreshExpires);

        _db.Users.Add(user);
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
