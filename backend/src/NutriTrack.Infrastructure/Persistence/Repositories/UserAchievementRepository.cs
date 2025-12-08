using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class UserAchievementRepository : EfRepository<UserAchievement, UserAchievementId>, IUserAchievementRepository
{
    public UserAchievementRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> ExistsAsync(UserId userId, string achievementKey, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.Achievement.Key == achievementKey, cancellationToken);
    }

    public async Task<PagedResult<UserAchievement>> GetPagedAsync(UserId userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.UserAchievements.AsQueryable();

        var totalCount = await query.CountAsync(ua => ua.UserId == userId, cancellationToken);

        var items = await query
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .OrderByDescending(ua => ua.LocalDateEarned)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserAchievement>(items, totalCount, page, pageSize);
    }
}