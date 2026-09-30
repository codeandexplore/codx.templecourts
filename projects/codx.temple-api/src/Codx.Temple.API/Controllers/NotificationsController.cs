using Codx.Temple.Application.DTOs.Notifications;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> List(
        [FromServices] ListNotificationsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkRead(
        Guid notificationId,
        [FromServices] MarkNotificationReadUseCase useCase,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(notificationId, ct);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(
        [FromServices] MarkAllNotificationsReadUseCase useCase,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(ct);
        return NoContent();
    }
}