using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IMealItemReadRepository
{
    Task<Dictionary<Guid, int>> CountByGroceryForUsersAsync(
        IReadOnlyCollection<UserId> userIds,
        CancellationToken cancellationToken = default);
}
