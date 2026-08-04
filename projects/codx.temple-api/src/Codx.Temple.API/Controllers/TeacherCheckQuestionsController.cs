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

    [HttpDelete("{id:guid}")]
    [RequireRole("Teacher")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
    {
        return NoContent();
    }
}
