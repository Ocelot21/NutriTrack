using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Api.Common.Clients;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public class NotificationsController : ApiController
{

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetForUser(
        [FromServices] NotificationsClient client,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool onlyUnread = false,
        CancellationToken cancellationToken = default)
    {
        var json = await client.GetForUserRaw(
            GetUserId().Value,
            onlyUnread,
            page,
            pageSize,
            cancellationToken);

        return Content(json, "application/json");
    }

    /* NOT USED YET
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(
        [FromServices] NotificationsClient client,
        CancellationToken cancellationToken = default)
    {
        var count = await client.GetUnreadCount(
            GetUserId().Value,
            cancellationToken);

        return Ok(count);
    }
    */

    [HttpPost("{notificationId:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkRead(
        [FromServices] NotificationsClient client,
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        await client.MarkRead(
            GetUserId().Value,
            notificationId,
            cancellationToken);

        return NoContent();
    }

    [HttpPost("mark-all-read")]
    [Authorize]
    public async Task<IActionResult> MarkAllRead(
        [FromServices] NotificationsClient client,
        CancellationToken cancellationToken = default)
    {
        await client.MarkAllRead(
            GetUserId().Value,
            cancellationToken);

        return NoContent();
    }
}
