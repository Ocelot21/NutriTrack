using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IUserAchievementRepository : IRepository<UserAchievement, UserAchievementId>
{
    Task<bool> ExistsAsync(UserId userId, string achievementKey, CancellationToken cancellationToken = default);
    Task<PagedResult<UserAchievement>> GetPagedAsync(
        UserId userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
