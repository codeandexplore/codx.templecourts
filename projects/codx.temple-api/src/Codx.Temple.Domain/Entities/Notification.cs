using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid RecipientId { get; private set; }
    public NotificationType Type { get; private set; }
    public string ReferenceType { get; private set; } = null!;
    public Guid ReferenceId { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }
    public DeliveryChannel DeliveryChannel { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User Recipient { get; private set; } = null!;

    private Notification() { }

    public static Notification Create(
        Guid recipientId,
        NotificationType type,
        string referenceType,
        Guid referenceId,
        DeliveryChannel deliveryChannel = DeliveryChannel.InApp)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = recipientId,
            Type = type,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            DeliveryChannel = deliveryChannel,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
