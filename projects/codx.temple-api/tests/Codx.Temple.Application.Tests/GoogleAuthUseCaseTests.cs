using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Codx.Temple.Application.Tests;

public class GoogleAuthUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<IGoogleAuthService> _googleMock;
    private readonly Mock<ITokenService> _tokenMock;
    private readonly GoogleAuthUseCase _useCase;

    public GoogleAuthUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _googleMock = new Mock<IGoogleAuthService>();
        _tokenMock = new Mock<ITokenService>();
        _useCase = new GoogleAuthUseCase(_dbMock.Object, _googleMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_NewGoogleUser_ShouldCreateAndReturnTokens()
    {
        var request = new GoogleAuthRequest("valid-id-token");
        _googleMock.Setup(g => g.ValidateIdTokenAsync("valid-id-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GoogleUserInfo("new@example.com", "New Google User", "google-id-123"));

        var users = new List<User>().AsQueryable();
        var userDbSet = DbSetMockHelper.CreateMockDbSet(users);
        _dbMock.Setup(db => db.Users).Returns(userDbSet.Object);

        _tokenMock.Setup(t => t.GenerateRefreshToken()).Returns(("rt", "rh", DateTimeOffset.UtcNow.AddDays(7)));
        _tokenMock.Setup(t => t.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns("at");

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal("at", result.AccessToken);
        Assert.Equal("new@example.com", result.User.Email);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidGoogleToken_ShouldThrow()
    {
        var request = new GoogleAuthRequest("invalid-token");
        _googleMock.Setup(g => g.ValidateIdTokenAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException("Invalid token"));

        await Assert.ThrowsAsync<UnauthorizedException>(() => _useCase.ExecuteAsync(request));
    }
}
