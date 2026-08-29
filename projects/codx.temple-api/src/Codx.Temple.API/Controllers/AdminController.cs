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

    [HttpDelete("role-assignments/{assignmentId:guid}")]
    public async Task<IActionResult> RevokeRole(
        Guid assignmentId,
        [FromServices] RevokeRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(assignmentId, cancellationToken);
        return NoContent();
    }

    [HttpGet("assignments")]
    public async Task<ActionResult<List<TeacherAssignmentDto>>> ListAssignments(
        [FromQuery] string? status,
        [FromServices] ListTeacherAssignmentsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(status, cancellationToken);
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> ListUsers(
        [FromServices] ListUsersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        Guid userId,
        [FromBody] ResetUserPasswordRequest request,
        [FromServices] ResetUserPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(userId, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("users/{userId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid userId,
        [FromBody] UpdateUserStatusRequest request,
        [FromServices] UpdateUserStatusUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(userId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("assignments/reassign")]
    public async Task<ActionResult<TeacherAssignmentDto>> Reassign(
        [FromBody] ReassignStudentRequest request,
        [FromServices] ReassignStudentUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }
}
