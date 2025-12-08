using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.ActivityLevelHistory;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IActivityLevelHistoryRepository : IRepository<ActivityLevelHistoryEntry, ActivityLevelHistoryId>
{
}
