using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Appointments;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    [HttpPost]
    [RequireRole("Teacher")]
    public async Task<ActionResult<AppointmentDto>> Create(
        [FromBody] CreateAppointmentRequest request,
        [FromServices] CreateAppointmentUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> List(
        [FromServices] ListAppointmentsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{appointmentId:guid}")]
    public async Task<ActionResult<AppointmentDto>> Get(
        Guid appointmentId,
        [FromServices] GetAppointmentUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(appointmentId, ct);
        return Ok(result);
    }

    [HttpPost("{appointmentId:guid}/confirm")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<AppointmentDto>> Confirm(
        Guid appointmentId,
        [FromServices] ConfirmAppointmentUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(appointmentId, ct);
        return Ok(result);
    }

    [HttpPost("{appointmentId:guid}/cancel")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<AppointmentDto>> Cancel(
        Guid appointmentId,
        [FromServices] CancelAppointmentUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(appointmentId, ct);
        return Ok(result);
    }
}
