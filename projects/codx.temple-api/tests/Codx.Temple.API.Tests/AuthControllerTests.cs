using Codx.Temple.API.Controllers;
using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.DTOs.Users;
using Codx.Temple.Application.Exceptions;
using Codx.Temple.Application.UseCases;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Codx.Temple.API.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAppDbContext> _dbMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new();
    private readonly Mock<ICurrentUserAccessor> _currentUserMock = new();

    private readonly Mock<RegisterUseCase> _registerUseCase;
    private readonly Mock<LoginUseCase> _loginUseCase;
    private readonly Mock<GoogleAuthUseCase> _googleAuthUseCase;
    private readonly Mock<RefreshTokenUseCase> _refreshTokenUseCase;
    private readonly Mock<GetCurrentUserUseCase> _getCurrentUserUseCase;

    private readonly Mock<IValidator<RegisterRequest>> _registerValidator = new();
    private readonly Mock<IValidator<LoginRequest>> _loginValidator = new();
    private readonly Mock<IValidator<GoogleAuthRequest>> _googleAuthValidator = new();

    public AuthControllerTests()
    {
        _registerUseCase = new Mock<RegisterUseCase>(_dbMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
        _loginUseCase = new Mock<LoginUseCase>(_dbMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
        _googleAuthUseCase = new Mock<GoogleAuthUseCase>(_dbMock.Object, _googleAuthServiceMock.Object, _tokenServiceMock.Object);
        _refreshTokenUseCase = new Mock<RefreshTokenUseCase>(_dbMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
        _getCurrentUserUseCase = new Mock<GetCurrentUserUseCase>(_dbMock.Object, _currentUserMock.Object);
    }

    private static AuthResponse CreateAuthResponse()
    {
        return new AuthResponse(
            "access-token",
            "refresh-token",
            DateTimeOffset.UtcNow,
            new UserInfo(Guid.NewGuid(), "test@example.com", "TestUser", new List<string> { "Student" }));
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        var request = new RegisterRequest("test@example.com", "password123", "TestUser");
        var expected = CreateAuthResponse();

        _registerUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        _registerValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<RegisterRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();
        var result = await controller.Register(request, _registerUseCase.Object, _registerValidator.Object, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Register_ShouldPropagateConflictException_WhenEmailExists()
    {
        var request = new RegisterRequest("test@example.com", "password123", "TestUser");

        _registerUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new ConflictException("A user with this email already exists."));

        _registerValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<RegisterRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();

        await FluentActions
            .Awaiting(() => controller.Register(request, _registerUseCase.Object, _registerValidator.Object, CancellationToken.None))
            .Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("test@example.com", "password123");
        var expected = CreateAuthResponse();

        _loginUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        _loginValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<LoginRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();
        var result = await controller.Login(request, _loginUseCase.Object, _loginValidator.Object, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Login_ShouldPropagateUnauthorizedException_WhenCredentialsAreInvalid()
    {
        var request = new LoginRequest("test@example.com", "wrong-password");

        _loginUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new UnauthorizedException("Invalid credentials."));

        _loginValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<LoginRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();

        await FluentActions
            .Awaiting(() => controller.Login(request, _loginUseCase.Object, _loginValidator.Object, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task GoogleAuth_ShouldReturnOk_WhenTokenIsValid()
    {
        var request = new GoogleAuthRequest("valid-google-id-token");
        var expected = CreateAuthResponse();

        _googleAuthUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        _googleAuthValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<GoogleAuthRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();
        var result = await controller.GoogleAuth(request, _googleAuthUseCase.Object, _googleAuthValidator.Object, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GoogleAuth_ShouldPropagateException_WhenTokenIsInvalid()
    {
        var request = new GoogleAuthRequest("invalid-token");

        _googleAuthUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new UnauthorizedException("Invalid Google token."));

        _googleAuthValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<GoogleAuthRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new AuthController();

        await FluentActions
            .Awaiting(() => controller.GoogleAuth(request, _googleAuthUseCase.Object, _googleAuthValidator.Object, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Refresh_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        var request = new RefreshTokenRequest("valid-refresh-token");
        var expected = CreateAuthResponse();

        _refreshTokenUseCase.Setup(u => u.ExecuteAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new AuthController();
        var result = await controller.Refresh(request, _refreshTokenUseCase.Object, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expected);
    }

    [Fact]
    public async Task Me_ShouldReturnOk_WhenUserExists()
    {
        var expected = new UserProfileDto(Guid.NewGuid(), "test@example.com", "TestUser", new List<string> { "Student" }, "Active");

        _getCurrentUserUseCase.Setup(u => u.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var controller = new AuthController();
        var result = await controller.Me(_getCurrentUserUseCase.Object, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expected);
    }
}
