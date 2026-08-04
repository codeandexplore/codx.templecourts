namespace Codx.Temple.Application.DTOs.Admin;

public record AssignRoleRequest(Guid UserId, string Role);

public record RoleAssignmentDto(Guid Id, Guid UserId, string UserEmail, string UserDisplayName, string Role, Guid AssignedBy, DateTimeOffset AssignedAt);
