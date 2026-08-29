using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using Codx.Temple.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Codx.Temple.Application.Tests;

public class RegisterUseCaseTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<ITokenService> _tokenMock;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _hasherMock = new Mock<IPasswordHasher>();
        _tokenMock = new Mock<ITokenService>();
        _useCase = new RegisterUseCase(_dbMock.Object, _hasherMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReturnAuthResponse()
    {
        var request = new RegisterRequest("test@example.com", "password123", "Test User");
        var users = new List<User>().AsQueryable();
        var userDbSet = DbSetMockHelper.CreateMockDbSet(users);
        _dbMock.Setup(db => db.Users).Returns(userDbSet.Object);
        var roleAssignments = new List<RoleAssignment>().AsQueryable();
        var roleDbSet = DbSetMockHelper.CreateMockDbSet(roleAssignments);
        _dbMock.Setup(db => db.RoleAssignments).Returns(roleDbSet.Object);

        _hasherMock.Setup(h => h.Hash(request.Password)).Returns("hashed-password");
        _tokenMock.Setup(t => t.GenerateRefreshToken()).Returns(("refresh-token", "refresh-hash", DateTimeOffset.UtcNow.AddDays(7)));
        _tokenMock.Setup(t => t.GenerateAccessToken(It.IsAny<Guid>(), "test@example.com", "Test User", It.IsAny<IEnumerable<string>>()))
            .Returns("access-token");

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal("Test User", result.User.DisplayName);
        Assert.Contains("Student", result.User.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateEmail_ShouldThrowConflict()
    {
        var request = new RegisterRequest("existing@example.com", "password123", "Test");
        var users = new List<User>
        {
            User.CreateWithPassword("existing@example.com", "old-hash", "Existing User")
        }.AsQueryable();
        var userDbSet = DbSetMockHelper.CreateMockDbSet(users);
        _dbMock.Setup(db => db.Users).Returns(userDbSet.Object);

        await Assert.ThrowsAsync<ConflictException>(() => _useCase.ExecuteAsync(request));
    }
}
