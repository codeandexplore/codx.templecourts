namespace Codx.Temple.Application.DTOs.Notifications;

public record NotificationDto(
    Guid Id,
    string Type,
    string ReferenceType,
    Guid ReferenceId,
    string DeliveryChannel,
    bool IsRead,
    DateTimeOffset CreatedAt);