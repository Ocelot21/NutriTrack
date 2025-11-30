using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IMealRepository : IRepository<Meal, MealId>
{
    Task<IReadOnlyList<Meal>> GetByUserAndDateRangeAsync(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
}
