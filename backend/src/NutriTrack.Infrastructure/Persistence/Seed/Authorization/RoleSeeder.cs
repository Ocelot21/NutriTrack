using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Persistence.Seed.Authorization;

public sealed class RoleSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public RoleSeeder(AppDbContext db)
    {
        _dbContext = db ?? throw new ArgumentNullException(nameof(db));
    }

    public int Order => 20;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var roles = new[]
        {
            new { Id = new RoleId(Guid.NewGuid()), Name = "Admin", Desc = "System administrator" },
            new { Id = new RoleId(Guid.NewGuid()), Name = "User", Desc = "End user" },
        };

        var existing = await _dbContext.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);

        var toAdd = roles
            .Where(r => !existingSet.Contains(r.Name))
            .Select(r =>
                Role.Create(r.Name, r.Desc, true)
            )
            .ToArray();

        if (toAdd.Length > 0)
        {
            _dbContext.Roles.AddRange(toAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
