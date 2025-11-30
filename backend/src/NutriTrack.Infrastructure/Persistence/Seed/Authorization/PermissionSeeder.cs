using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Persistence.Seed.Authorization;

public sealed class PermissionSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public PermissionSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 10;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var existingKeys = await _dbContext.Permissions
            .Select(p => p.Key.Value)
            .ToListAsync(cancellationToken);

        var missing = PermissionKeys.All
            .Except(existingKeys, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (missing.Length > 0)
        {
            var toAdd = missing.Select(key =>
                Permission.Create(key, key));

            _dbContext.Permissions.AddRange(toAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }

}
