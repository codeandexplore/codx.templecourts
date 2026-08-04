using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("api/lessons/{lessonKey:guid}/versions")]
[RequireRole("Admin")]
public class LessonVersionsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LessonVersionDto>>> List(
        Guid lessonKey,
        [FromServices] ListVersionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(lessonKey, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LessonVersionDto>> CreateDraft(
        Guid lessonKey,
        [FromBody] CreateLessonVersionRequest request,
        [FromServices] CreateDraftVersionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(lessonKey, request, cancellationToken);
        return CreatedAtAction(nameof(List), new { lessonKey }, result);
    }

    [HttpPost("{versionId:guid}/publish")]
    public async Task<IActionResult> Publish(
        Guid lessonKey,
        Guid versionId,
        [FromServices] PublishVersionUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(lessonKey, versionId, cancellationToken);
        return NoContent();
    }
}
