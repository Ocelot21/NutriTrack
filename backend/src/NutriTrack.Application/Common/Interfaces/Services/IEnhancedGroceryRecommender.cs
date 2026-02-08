using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Services;

public interface IEnhancedGroceryRecommender
{
    Task<PagedResult<GroceryRecommendationResult>> GetRecommendedAsync(
        UserId userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
