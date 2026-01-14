using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Notifications.Api.Contracts.Notifications;
using NutriTrack.Notifications.Application.Notifications.Commands.MarkAllNotificationsAsRead;
using NutriTrack.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;
using NutriTrack.Notifications.Application.Notifications.Queries.GetUnreadNotificationCount;
using NutriTrack.Notifications.Application.Notifications.Queries.GetUserNotifications;

namespace NutriTrack.Notifications.Api.Controllers;

[Route("internal/users/{userId:guid}/notifications")]
public class InternalNotificationsController : ApiController
{
    private readonly ISender _mediator;

    public InternalNotificationsController(ISender mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetForUser(
        Guid userId,
        [FromQuery] bool onlyUnread = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserNotificationsQuery(userId, page, pageSize, onlyUnread);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            notifications => Ok(notifications),
            errors => Problem(errors));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUnreadNotificationCountQuery(userId);

        var count = await _mediator.Send(query, cancellationToken);

        return Ok(new UnreadCountResponse(count));
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkRead(
        Guid userId,
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkNotificationAsReadCommand(userId, notificationId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkAllNotificationsAsReadCommand(userId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}