using Codx.Temple.API.Authorization;
using Codx.Temple.Application.DTOs.Admin;
using Codx.Temple.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codx.Temple.API.Controllers;

[ApiController]
[Authorize]
public class TeacherAssignmentsController : ControllerBase
{
    [HttpPost("api/students/{studentId:guid}/claim")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<TeacherAssignmentDto>> Claim(
        Guid studentId,
        [FromServices] ClaimStudentUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(studentId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/teacher/students")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<List<TeacherAssignmentDto>>> ListStudents(
        [FromServices] GetTeacherStudentsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("api/students/unassigned")]
    [RequireRole("Teacher")]
    public async Task<ActionResult<List<UserDto>>> ListUnassignedStudents(
        [FromServices] ListUnassignedStudentsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }
}
