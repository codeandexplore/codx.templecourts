using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("api/lesson-versions/{versionId:guid}/nodes")]
[RequireRole("Admin")]
public class LessonNodesController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LessonNodeDto>> Add(
        Guid versionId,
        [FromBody] CreateLessonNodeRequest request,
        [FromServices] AddLessonNodeUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(versionId, request, cancellationToken);
        return CreatedAtAction(nameof(Add), new { versionId }, result);
    }

    [HttpPut("{nodeKey:guid}")]
    public async Task<ActionResult<LessonNodeDto>> Update(
        Guid versionId,
        Guid nodeKey,
        [FromBody] UpdateLessonNodeRequest request,
        [FromServices] UpdateLessonNodeUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(versionId, nodeKey, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{nodeKey:guid}")]
    public async Task<IActionResult> Delete(
        Guid versionId,
        Guid nodeKey,
        [FromServices] DeleteLessonNodeUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(versionId, nodeKey, cancellationToken);
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(
        Guid versionId,
        [FromBody] ReorderNodesRequest request,
        [FromServices] ReorderNodesUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(versionId, request, cancellationToken);
        return NoContent();
    }
}
