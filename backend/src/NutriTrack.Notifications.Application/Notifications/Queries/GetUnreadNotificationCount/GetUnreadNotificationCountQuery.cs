using MediatR;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUnreadNotificationCount;

public sealed record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<int>;
