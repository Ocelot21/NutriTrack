using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Authorization;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Infrastructure.Authorization;

public sealed class PermissionProvider : IPermissionProvider
{
    private readonly AppDbContext _dbContext;
    public PermissionProvider(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<string>> GetForUserAsync(UserId userId, CancellationToken ct = default)
    {
        var keys = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.RoleId)
                .First())
            .Select(rp => rp.Permission.Key.Value)
            .ToListAsync(ct);

        return keys.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public async Task<string?> GetRoleNameAsync(UserId userId, CancellationToken ct = default)
    {
        return await _dbContext.Users
        .Where(u => u.Id == userId)
        .Join(
            _dbContext.Roles,
            u => u.RoleId,
            r => r.Id,
            (u, r) => r.Name
        ).FirstOrDefaultAsync(ct);
    }
}