using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Persistence.Seed.Authorization;

public sealed class RolePermissionSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public RolePermissionSeeder(AppDbContext db)
    {
        _dbContext = db ?? throw new ArgumentNullException(nameof(db));
    }

    public int Order => 30;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);

        var roles = await _dbContext.Roles
            .Select(r => new { r.Name, r.Id })
            .ToListAsync(ct);

        var roleByName = roles.ToDictionary(
            r => r.Name,
            r => r.Id,
            StringComparer.OrdinalIgnoreCase);

        if (!roleByName.TryGetValue("Admin", out var adminRoleId))
            throw new InvalidOperationException("Admin role not found, run RoleSeeder first!");

        if (!roleByName.TryGetValue("User", out var userRoleId))
            throw new InvalidOperationException("Customer role not found, run RoleSeeder first!");



        var permissions = await _dbContext.Permissions
            .Select(p => new { p.Key, p.Id })
            .ToListAsync(ct);

        var permByKey = permissions.ToDictionary(
            p => p.Key.Value,
            p => p.Id,
            StringComparer.OrdinalIgnoreCase);

        await AssignMissing(adminRoleId, AuthorizationProfiles.AdminAll, permByKey, ct);
        await AssignMissing(userRoleId, AuthorizationProfiles.UserBasic, permByKey, ct);

        await tx.CommitAsync(ct);
    }


    private async Task AssignMissing(
        RoleId roleId,
        IEnumerable<string> keys,
        IDictionary<string, PermissionId> permByKey,
        CancellationToken ct)
    {
        var targetPermissionIds = keys
            .Where(permByKey.ContainsKey)
            .Select(k => permByKey[k])
            .ToHashSet();

        var existing = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);

        var existingSet = new HashSet<PermissionId>(existing);
        var missing = targetPermissionIds.Except(existingSet).ToArray();

        if (missing.Length == 0)
            return;

        foreach (var permId in missing)
        {
            _dbContext.RolePermissions.Add(
                new RolePermission()
                {
                    RoleId = roleId,
                    PermissionId = permId,
                }
            );
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
