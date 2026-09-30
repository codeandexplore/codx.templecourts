namespace Codx.Temple.Application.DTOs.Admin;

public record AuditLogDto(
    Guid Id,
    string Action,
    Guid PerformedById,
    string PerformedByDisplayName,
    Guid? TargetUserId,
    string? TargetUserDisplayName,
    string? Metadata,
    DateTimeOffset CreatedAt);