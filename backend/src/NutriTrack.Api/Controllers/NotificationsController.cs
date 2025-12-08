using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Notifications;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public class NotificationsController : ApiController
{
    private readonly IUserNotificationsReadRepository _userNotificationsReadRepository;

    public NotificationsController(IUserNotificationsReadRepository userNotificationsReadRepository)
    {
        _userNotificationsReadRepository = userNotificationsReadRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool onlyUnread = false,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();

        var result = await _userNotificationsReadRepository.GetForCurrentUserAsync(
            userId,
            page,
            pageSize,
            onlyUnread,
            cancellationToken);

        var response = new PagedResponse<NotificationResponse>(
            result.Items.Select(n => new NotificationResponse(
                n.Id,
                n.UserId,
                n.Title,
                n.Message,
                n.Type.ToString(),
                n.Status.ToString(),
                n.OccurredAtUtc.ToString("o"),
                n.ReadAtUtc?.ToString("o") ?? string.Empty,
                n.LinkUrl,
                n.MetadataJson)).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount);

        return Ok(response);
    }
}
