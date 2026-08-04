using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Route("admin")]
[RequireRole("Admin")]
public class AdminController : ControllerBase
{
    [HttpGet("role-assignments")]
    public async Task<ActionResult<List<RoleAssignmentDto>>> ListRoleAssignments(
        [FromServices] ListRoleAssignmentsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("role-assignments")]
    public async Task<ActionResult<RoleAssignmentDto>> AssignRole(
        [FromBody] AssignRoleRequest request,
        [FromServices] AssignRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ListRoleAssignments), null, result);
    }
}
