using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Common.Interfaces.Messaging;
using NutriTrack.Application.Notifications.Messages;

namespace NutriTrack.Api.Controllers
{
    [Route("api/dev-notifications")]
    [ApiController]
    public class DevNotificationsController : ControllerBase
    {
        private readonly INotificationPublisher _publisher;

        public DevNotificationsController(INotificationPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification()
        {
            var userId = Guid.NewGuid();

            var message = new NotificationRequestedMessage(
                userId,
                "Test notification",
                "This is a test message from the main API.",
                "System",
                OccurredAtUtc: DateTime.UtcNow,
                LinkUrl: null,
                MetadataJson: null);

            await _publisher.PublishAsync(message);

            return Ok(new { SentTo = userId });
        }
    }
}
