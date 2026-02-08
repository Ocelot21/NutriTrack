using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common.Errors;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _dbContext;

    public RolePermissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<string>> ListPermissionKeysByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        var keys = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission.Key.Value)
            .ToListAsync(cancellationToken);

        return keys.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public async Task<bool> AddAsync(RoleId roleId, string permissionKey, CancellationToken cancellationToken = default)
    {
        var keyRaw = (permissionKey ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(keyRaw))
        {
            throw new DomainException(DomainErrorCodes.Authorization.InvalidPermissionKey, "Permission key cannot be empty.");
        }

        var pk = PermissionKey.Create(keyRaw);

        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Key == pk, cancellationToken);

        if (permission is null)
        {
            return false;
        }

        var exists = await _dbContext.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id, cancellationToken);

        if (exists)
        {
            return true;
        }

        _dbContext.RolePermissions.Add(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permission.Id,
        });

        return true;
    }

    public async Task<bool> RemoveAsync(RoleId roleId, string permissionKey, CancellationToken cancellationToken = default)
    {
        var keyRaw = (permissionKey ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(keyRaw))
        {
            throw new DomainException(DomainErrorCodes.Authorization.InvalidPermissionKey, "Permission key cannot be empty.");
        }

        var pk = PermissionKey.Create(keyRaw);

        var permissionId = await _dbContext.Permissions
            .Where(p => p.Key == pk)
            .Select(p => (PermissionId?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (permissionId is null)
        {
            return false;
        }

        var rolePermission = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);

        if (rolePermission is null)
        {
            return true;
        }

        _dbContext.RolePermissions.Remove(rolePermission);
        return true;
    }
}
