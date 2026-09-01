using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/check-questions")]
public class TeacherCheckQuestionsController : ControllerBase
{
    [HttpPost]
    [RequireRole("Teacher")]
    public async Task<ActionResult<TeacherCheckQuestionDto>> Create(
        [FromBody] CreateCheckQuestionRequest request,
        [FromServices] CreateCheckQuestionUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("questions")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<List<PublishedQuestionDto>>> ListQuestions(
        [FromServices] ListPublishedQuestionsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet]
    [RequireRole("Teacher")]
    public async Task<ActionResult<List<TeacherCheckQuestionDto>>> List(
        [FromServices] ListCheckQuestionsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<TeacherCheckQuestionDto>> Get(
        Guid id,
        [FromServices] GetCheckQuestionUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<TeacherCheckQuestionDto>> Update(
        Guid id,
        [FromBody] UpdateCheckQuestionRequest request,
        [FromServices] UpdateCheckQuestionUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequireRole("Teacher")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] DeleteCheckQuestionUseCase useCase,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }
}