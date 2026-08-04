using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    [HttpGet]
    [RequireRole("Admin")]
    public async Task<ActionResult<List<LessonDto>>> List(
        [FromServices] ListLessonsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{key:guid}")]
    [Authorize]
    public async Task<ActionResult<LessonVersionDto>> Get(
        Guid key,
        [FromServices] GetLessonTreeUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(key, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RequireRole("Admin")]
    public async Task<ActionResult<LessonDto>> Create(
        [FromBody] CreateLessonRequest request,
        [FromServices] CreateLessonUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { key = result.Key }, result);
    }

    [HttpPut("{key:guid}")]
    [RequireRole("Admin")]
    public async Task<ActionResult<LessonDto>> Update(
        Guid key,
        [FromBody] UpdateLessonRequest request,
        [FromServices] UpdateLessonUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(key, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{key:guid}")]
    [RequireRole("Admin")]
    public async Task<IActionResult> Archive(
        Guid key,
        [FromServices] ArchiveLessonUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(key, cancellationToken);
        return NoContent();
    }
}
