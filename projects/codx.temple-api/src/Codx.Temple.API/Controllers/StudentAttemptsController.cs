using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
public class StudentAttemptsController : ControllerBase
{
    [HttpPost("api/lessons/{lessonKey:guid}/attempts")]
    public async Task<ActionResult<LessonAttemptDto>> Start(
        Guid lessonKey,
        [FromServices] StartLessonAttemptUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(lessonKey, cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/attempts/{attemptId:guid}")]
    public async Task<ActionResult<LessonAttemptDto>> Get(
        Guid attemptId,
        [FromServices] GetAttemptUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(attemptId, cancellationToken);
        return Ok(result);
    }
}
