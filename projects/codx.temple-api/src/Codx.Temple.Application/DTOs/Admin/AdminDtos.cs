namespace Codx.Temple.Application.DTOs.Admin;

public record AssignRoleRequest(Guid UserId, string Role);

public record RoleAssignmentDto(Guid Id, Guid UserId, string UserEmail, string UserDisplayName, string Role, Guid AssignedBy, DateTimeOffset AssignedAt);

public record ReassignStudentRequest(Guid StudentId, Guid NewTeacherId);

public record TeacherAssignmentDto(
    Guid Id,
    Guid StudentId,
    string StudentEmail,
    string StudentDisplayName,
    Guid PrimaryTeacherId,
    string PrimaryTeacherEmail,
    string PrimaryTeacherDisplayName,
    string Status,
    DateTimeOffset AssignedAt,
    DateTimeOffset? EndedAt);
