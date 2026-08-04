using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/study-sessions")]
public class StudySessionsController : ControllerBase
{
    [HttpPost]
    [RequireRole("Teacher")]
    public async Task<ActionResult<StudySessionDto>> Start(
        [FromBody] StartSessionRequest request,
        [FromServices] StartStudySessionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{sessionId:guid}/advance")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<StudySessionDto>> Advance(
        Guid sessionId,
        [FromBody] AdvanceSessionRequest request,
        [FromServices] AdvanceStudySessionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{sessionId:guid}/end")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<StudySessionDto>> End(
        Guid sessionId,
        [FromServices] EndStudySessionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<StudySessionDto>> Get(
        Guid sessionId,
        [FromServices] GetStudySessionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("mark-reviewed")]
    [RequireRole("Teacher")]
    public async Task<IActionResult> MarkReviewed(
        [FromBody] MarkReviewedRequest request,
        [FromServices] MarkAnswerReviewedUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.MarkReviewedAsync(request, cancellationToken);
        return NoContent();
    }
}
