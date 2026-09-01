using Codx.Temple.API.Hubs;
using Codx.Temple.Application.DTOs.Communication;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/threads")]
public class AnswerThreadsController : ControllerBase
{
    [HttpGet("{threadId:guid}")]
    public async Task<ActionResult<AnswerThreadDto>> Get(
        Guid threadId,
        [FromServices] GetThreadUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(threadId, ct);
        return Ok(result);
    }

    [HttpPost("{threadId:guid}/messages")]
    public async Task<ActionResult<ThreadMessageDto>> PostMessage(
        Guid threadId,
        [FromBody] PostMessageRequest request,
        [FromServices] PostThreadMessageUseCase useCase,
        [FromServices] IHubContext<StudySessionHub> hubContext,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(threadId, request, ct);
        await hubContext.Clients.Group($"thread:{threadId}").SendAsync("ThreadMessagePosted", new { threadId }, ct);
        return Ok(result);
    }
}
