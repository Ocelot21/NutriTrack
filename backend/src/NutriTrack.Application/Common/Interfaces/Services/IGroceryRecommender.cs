using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Services;

public interface IGroceryRecommender
{
    Task<PagedResult<GroceryResult>> GetRecommendedAsync(
        UserId userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
