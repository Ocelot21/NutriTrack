using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class UserGoalRepository : EfRepository<UserGoal, UserGoalId>, IUserGoalRepository
{
    public UserGoalRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedResult<UserGoal>> GetPagedAsync(UserId userId, int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _dbContext.UserGoals.Where(g => g.UserId == userId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(g => g.TargetDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<UserGoal>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<UserGoal>> ListInProgress(UserId userId)
    {
        return await _dbContext.UserGoals.Where(
            g => g.UserId == userId &&
            g.Status == UserGoalStatus.InProgress).ToListAsync();
    }

    public async Task<UserGoal?> GetCurrentForUser(UserId userId)
    {
        return await _dbContext.UserGoals.FirstOrDefaultAsync(
            g => g.UserId == userId 
        && g.Status == UserGoalStatus.InProgress);
    }

    public async Task<int> CountCompletedAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserGoals.CountAsync(
            g => g.UserId == userId && g.Status == UserGoalStatus.Completed, cancellationToken);
    }
}