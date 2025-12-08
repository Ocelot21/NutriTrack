using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.ActivityLevelHistory;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class ActivityLevelHistoryRepository : EfRepository<ActivityLevelHistoryEntry, ActivityLevelHistoryId>, IActivityLevelHistoryRepository
{
    public ActivityLevelHistoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
