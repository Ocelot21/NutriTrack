using NutriTrack.Application.Notifications.Messages;

namespace NutriTrack.Application.Common.Interfaces.Messaging;

public interface INotificationPublisher
{
    Task PublishAsync(NotificationRequestedMessage message, CancellationToken cancellationToken = default);
}
