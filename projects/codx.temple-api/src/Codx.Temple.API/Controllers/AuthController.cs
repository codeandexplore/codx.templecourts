using Codx.Temple.Application.Abstractions;
using Codx.Temple.Application.DTOs.Auth;
using Codx.Temple.Application.UseCases;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterUseCase useCase,
        [FromServices] IValidator<RegisterRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginUseCase useCase,
        [FromServices] IValidator<LoginRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> GoogleAuth(
        [FromBody] GoogleAuthRequest request,
        [FromServices] GoogleAuthUseCase useCase,
        [FromServices] IValidator<GoogleAuthRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        [FromServices] RefreshTokenUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(
        [FromServices] GetCurrentUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }
}
