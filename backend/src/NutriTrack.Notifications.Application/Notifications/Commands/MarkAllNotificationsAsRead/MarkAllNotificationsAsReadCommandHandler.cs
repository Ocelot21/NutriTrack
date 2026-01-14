using ErrorOr;
using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Common.Interfaces.Services;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand, ErrorOr<Unit>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId, cancellationToken);

        var utcNow = _dateTimeProvider.UtcNow;

        foreach (var notification in notifications)
        {
            notification.MarkAsRead(utcNow);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
