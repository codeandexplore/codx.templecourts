using Codx.Temple.Application.DTOs.StudentAttempts;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/attempts/{attemptId:guid}/answers")]
public class StudentAnswersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StudentAnswerDto>> Submit(
        Guid attemptId,
        [FromBody] SubmitAnswerRequest request,
        [FromServices] SubmitAnswerUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(attemptId, request, cancellationToken);
        return Ok(result);
    }
}
