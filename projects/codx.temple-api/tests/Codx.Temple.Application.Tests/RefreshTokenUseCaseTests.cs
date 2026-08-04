using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Moq;

namespace Codx.Temple.Application.Tests;

public class RefreshTokenUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<ITokenService> _tokenMock;
    private readonly RefreshTokenUseCase _useCase;

    public RefreshTokenUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenMock = new Mock<ITokenService>();
        _useCase = new RefreshTokenUseCase(_dbMock.Object, _hasherMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoUsers_ShouldThrowUnauthorized()
    {
        var request = new RefreshTokenRequest("any-token");
        var data = new List<Domain.Entities.User>();
        var dbSet = DbSetMockHelper.CreateMockDbSet(data);
        _dbMock.Setup(db => db.Users).Returns(dbSet.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => _useCase.ExecuteAsync(request));
    }
}
