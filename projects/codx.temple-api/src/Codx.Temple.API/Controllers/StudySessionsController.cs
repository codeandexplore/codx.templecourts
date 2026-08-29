using Codx.Temple.API.Authorization;
using Codx.Temple.API.Hubs;
using Codx.Temple.Application.DTOs.StudySessions;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
        [FromServices] IHubContext<StudySessionHub> hubContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, request, cancellationToken);
        await hubContext.Clients.Group(sessionId.ToString()).SendAsync("SessionAdvanced", new { result.CurrentQuestionId }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{sessionId:guid}/end")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<StudySessionDto>> End(
        Guid sessionId,
        [FromServices] EndStudySessionUseCase useCase,
        [FromServices] IHubContext<StudySessionHub> hubContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, cancellationToken);
        await hubContext.Clients.Group(sessionId.ToString()).SendAsync("SessionEnded", new { }, cancellationToken);
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
        [FromServices] IHubContext<StudySessionHub> hubContext,
        [FromServices] Infrastructure.Data.AppDbContext db,
        CancellationToken cancellationToken)
    {
        await useCase.MarkReviewedAsync(request, cancellationToken);

        var activeSessionId = await db.StudySessions
            .Where(s => s.LessonAttemptId == request.LessonAttemptId
                && s.Status == Domain.Enums.StudySessionStatus.InProgress)
            .Select(s => (Guid?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeSessionId.HasValue)
        {
            await hubContext.Clients.Group(activeSessionId.Value.ToString())
                .SendAsync("QuestionReviewed", new { request.QuestionKey, IsReviewed = true }, cancellationToken);
        }

        return NoContent();
    }

    [HttpGet("{sessionId:guid}/questions")]
    public async Task<ActionResult<SessionQuestionsDto>> GetQuestions(
        Guid sessionId,
        [FromServices] GetSessionQuestionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(sessionId, cancellationToken);
        return Ok(result);
    }
}
