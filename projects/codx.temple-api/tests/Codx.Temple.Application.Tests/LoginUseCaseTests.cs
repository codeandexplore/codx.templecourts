using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Moq;

namespace Codx.Temple.Application.Tests;

public class LoginUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<ITokenService> _tokenMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenMock = new Mock<ITokenService>();
        _useCase = new LoginUseCase(_dbMock.Object, _hasherMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCredentials_ShouldThrowUnauthorized()
    {
        var request = new LoginRequest("test@example.com", "wrongpassword");
        var data = new List<User>().AsQueryable();
        var dbSet = DbSetMockHelper.CreateMockDbSet(data);
        _dbMock.Setup(db => db.Users).Returns(dbSet.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => _useCase.ExecuteAsync(request));
    }
}
