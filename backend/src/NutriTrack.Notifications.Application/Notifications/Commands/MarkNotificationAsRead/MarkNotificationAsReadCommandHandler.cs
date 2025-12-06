using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Common.Interfaces.Services;
using NutriTrack.Notifications.Domain.Notifications;

namespace NutriTrack.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler
    : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _notificationRepository = notificationRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var id = new NotificationId(request.NotificationId);

        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);

        if (notification is null)
        {
            // TODO: Error handling - notification not found
            return;
        }

        notification.MarkAsRead(_dateTimeProvider.UtcNow);

        await Task.CompletedTask;
    }
}