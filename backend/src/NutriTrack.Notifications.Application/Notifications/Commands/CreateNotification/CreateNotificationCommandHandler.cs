using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Common.Interfaces.Services;
using NutriTrack.Notifications.Domain.Notifications;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Notifications.Commands.CreateNotification;

public sealed class CreateNotificationCommandHandler
    : IRequestHandler<CreateNotificationCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var occurredAtUtc = request.OccurredAtUtc == default
            ? _dateTimeProvider.UtcNow
            : request.OccurredAtUtc;

        var notification = Notification.Create(
            userId,
            request.Title,
            request.Message,
            request.Type,
            occurredAtUtc,
            request.LinkUrl,
            request.MetadataJson);

        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken); 
    }
}
