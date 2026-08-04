using Codx.Temple.Application.DTOs.StudentNotes;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/notes")]
public class StudentNotesController : ControllerBase
{
    [HttpPost("{questionKey:guid}")]
    public async Task<ActionResult<StudentQuestionNoteDto>> Create(
        Guid questionKey,
        [FromBody] CreateNoteRequest request,
        [FromServices] CreateStudentQuestionNoteUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(questionKey, request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<StudentQuestionNoteDto>>> List(
        [FromServices] ListStudentQuestionNotesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{questionKey:guid}")]
    public async Task<ActionResult<StudentQuestionNoteDto>> Get(
        Guid questionKey,
        [FromServices] GetStudentQuestionNoteUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(questionKey, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{questionKey:guid}")]
    public async Task<ActionResult<StudentQuestionNoteDto>> Update(
        Guid questionKey,
        [FromBody] UpdateNoteRequest request,
        [FromServices] UpdateStudentQuestionNoteUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(questionKey, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{questionKey:guid}")]
    public async Task<IActionResult> Delete(
        Guid questionKey,
        [FromServices] DeleteStudentQuestionNoteUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(questionKey, cancellationToken);
        return NoContent();
    }
}
