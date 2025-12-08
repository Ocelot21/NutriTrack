using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Notifications;
using NutriTrack.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infrastructure.Notifications.Persistence
{
    public class UserNotificationReadRepository : IUserNotificationsReadRepository
    {
        private readonly NotificationsReadDbContext _dbContext;

        public UserNotificationReadRepository(NotificationsReadDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<NotificationReadModel>> GetForCurrentUserAsync(
            UserId userId,
            int page,
            int pageSize,
            bool onlyUnread,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Notifications.Where(n => n.UserId == userId.Value).AsQueryable();

            if (onlyUnread)
            {
                query = query.Where(n => n.ReadAtUtc == null);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(n => n.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<NotificationReadModel>(items.Select(n => new NotificationReadModel
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status,
                OccurredAtUtc = n.OccurredAtUtc,
                ReadAtUtc = n.ReadAtUtc,
                LinkUrl = n.LinkUrl,
                MetadataJson = n.MetadataJson
            }).ToList(), totalCount, page, pageSize);
        }
    }
}
