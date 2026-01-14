using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class SocialPostRepository : EfRepository<SocialPost, SocialPostId>, ISocialPostRepository
{
    public SocialPostRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedResult<SocialPost>> GetFeedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _dbContext.SocialPosts
            .Include(p => p.UserAchievement)
                .ThenInclude(ua => ua!.Achievement)
            .Include(p => p.DailyOverviewSnapshot)
            .Include(p => p.GoalProgressSnapshot)
            .OrderByDescending(p => p.CreatedAtUtc)
            .AsQueryable();


        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SocialPost>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<SocialPost>> ListByUserAsync(
        UserId userId,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0) take = 20;

        return await _dbContext.SocialPosts
            .Where(p => p.UserId == userId)
            .Include(p => p.UserAchievement)
                .ThenInclude(ua => ua!.Achievement)
            .Include(p => p.DailyOverviewSnapshot)
            .Include(p => p.GoalProgressSnapshot)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
