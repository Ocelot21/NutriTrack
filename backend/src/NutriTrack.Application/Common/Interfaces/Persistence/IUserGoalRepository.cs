using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IUserGoalRepository : IRepository<UserGoal, UserGoalId>
{
    Task<PagedResult<UserGoal>> GetPagedAsync(UserId userId, int Page, int PageSize);
    Task<IReadOnlyList<UserGoal>> ListInProgress(UserId userId);
    Task<UserGoal?> GetCurrentForUser(UserId userId);
    Task<int> CountCompletedAsync(UserId userId, CancellationToken cancellationToken = default);
}