using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class ActivityLevelHistoryRepository : EfRepository<ActivityLevelHistoryEntry, ActivityLevelHistoryId>, IActivityLevelHistoryRepository
{
    public ActivityLevelHistoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ActivityLevelHistoryEntry?> GetClosestOnOrBeforeAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ActivityLevelHistoryEntries
            .Where(e => e.UserId == userId && e.EffectiveFrom <= date)
            .OrderByDescending(e => e.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
