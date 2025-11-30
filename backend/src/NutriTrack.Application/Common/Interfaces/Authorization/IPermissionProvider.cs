using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Authorization;

public interface IPermissionProvider
{
    Task<IReadOnlyCollection<string>> GetForUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<string?> GetRoleNameAsync(UserId userId, CancellationToken cancellationToken = default);
}
