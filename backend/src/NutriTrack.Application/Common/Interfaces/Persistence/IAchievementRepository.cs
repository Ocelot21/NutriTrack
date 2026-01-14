using NutriTrack.Domain.Achievements;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IAchievementRepository : IRepository<Achievement, AchievementId>
{
    Task<Achievement?> GetByKeyAsync(
        string key, 
        CancellationToken cancellationToken = default);
}
