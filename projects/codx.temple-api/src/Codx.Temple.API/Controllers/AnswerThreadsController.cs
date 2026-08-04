using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/threads")]
public class AnswerThreadsController : ControllerBase
{
    [HttpPost("{threadId:guid}/messages")]
    public async Task<ActionResult<ThreadMessageDto>> PostMessage(
        Guid threadId,
        [FromBody] PostMessageRequest request,
        [FromServices] PostThreadMessageUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(threadId, request, ct);
        return Ok(result);
    }
}
