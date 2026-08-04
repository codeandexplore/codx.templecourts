using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Lessons;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("api/lesson-nodes/{nodeKey:guid}/questions")]
[RequireRole("Admin")]
public class QuestionsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Add(
        Guid nodeKey,
        [FromBody] CreateQuestionRequest request,
        [FromServices] AddQuestionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(nodeKey, request, cancellationToken);
        return CreatedAtAction(nameof(Add), new { nodeKey }, result);
    }

    [HttpPut("{questionKey:guid}")]
    public async Task<ActionResult<QuestionDto>> Update(
        Guid nodeKey,
        Guid questionKey,
        [FromBody] UpdateQuestionRequest request,
        [FromServices] UpdateQuestionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(nodeKey, questionKey, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{questionKey:guid}")]
    public async Task<IActionResult> Delete(
        Guid nodeKey,
        Guid questionKey,
        [FromServices] DeleteQuestionUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(nodeKey, questionKey, cancellationToken);
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(
        Guid nodeKey,
        [FromBody] ReorderQuestionsRequest request,
        [FromServices] ReorderQuestionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(nodeKey, request, cancellationToken);
        return NoContent();
    }
}
