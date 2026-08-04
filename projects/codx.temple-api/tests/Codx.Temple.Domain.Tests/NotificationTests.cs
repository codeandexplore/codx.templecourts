using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;

namespace Codx.Temple.Domain.Tests;

public class NotificationTests
{
    [Fact]
    public void Create_ShouldSetAllFields()
    {
        var recipientId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var notification = Notification.Create(
            recipientId,
            NotificationType.NewThreadMessage,
            "AnswerThread",
            referenceId,
            DeliveryChannel.InApp);

        Assert.NotEqual(Guid.Empty, notification.Id);
        Assert.Equal(recipientId, notification.RecipientId);
        Assert.Equal(NotificationType.NewThreadMessage, notification.Type);
        Assert.Equal("AnswerThread", notification.ReferenceType);
        Assert.Equal(referenceId, notification.ReferenceId);
        Assert.Equal(DeliveryChannel.InApp, notification.DeliveryChannel);
        Assert.Null(notification.ReadAt);
    }

    [Fact]
    public void Create_ShouldDefaultToInAppChannel()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            NotificationType.SessionStarted,
            "StudySession",
            Guid.NewGuid());

        Assert.Equal(DeliveryChannel.InApp, notification.DeliveryChannel);
    }
}
