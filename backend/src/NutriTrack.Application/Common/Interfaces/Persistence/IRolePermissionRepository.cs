using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<string>> ListPermissionKeysByRoleIdAsync(
        RoleId roleId,
        CancellationToken cancellationToken = default);

    Task<bool> AddAsync(
        RoleId roleId,
        string permissionKey,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(
        RoleId roleId,
        string permissionKey,
        CancellationToken cancellationToken = default);
}
