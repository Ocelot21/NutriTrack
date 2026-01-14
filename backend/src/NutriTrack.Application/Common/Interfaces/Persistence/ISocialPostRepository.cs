using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface ISocialPostRepository : IRepository<SocialPost, SocialPostId>
{
    Task<PagedResult<SocialPost>> GetFeedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SocialPost>> ListByUserAsync(
        UserId userId,
        int take,
        CancellationToken cancellationToken = default);
}
