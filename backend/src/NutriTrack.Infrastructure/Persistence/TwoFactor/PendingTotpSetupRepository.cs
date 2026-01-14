using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.TwoFactor.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.TwoFactor;

public sealed class PendingTotpSetupRepository : IPendingTotpSetupRepository
{
    private readonly AppDbContext _dbContext;

    public PendingTotpSetupRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(PendingTotpSetup setup, CancellationToken cancellationToken)
    {
        var existing = await GetEntityAsync(setup.UserId, cancellationToken);

        if (existing is null)
        {
            var setupEntity = new PendingTotpSetupEntity(
                setup.UserId,
                setup.SecretProtected,
                setup.ExpiresAtUtc);

            await _dbContext.Set<PendingTotpSetupEntity>().AddAsync(setupEntity, cancellationToken);
            return;
        }

        typeof(PendingTotpSetupEntity).GetProperty(nameof(PendingTotpSetupEntity.SecretProtected))!
            .SetValue(existing, setup.SecretProtected);

        typeof(PendingTotpSetupEntity).GetProperty(nameof(PendingTotpSetupEntity.ExpiresAtUtc))!
            .SetValue(existing, setup.ExpiresAtUtc);
    }

    public async Task DeleteAsync(UserId userId, CancellationToken cancellationToken)
    {
        var existing = await GetEntityAsync(userId, cancellationToken);

        if (existing is null) return;

        _dbContext.PendingTotpSetups.Remove(existing);
    }

    public async Task<PendingTotpSetup?> GetAsync(UserId userId, CancellationToken cancellationToken)
    {
        var entity = await GetEntityAsync(userId, cancellationToken);

        return entity is null
            ? null
            : new PendingTotpSetup(entity.UserId, entity.SecretProtected, entity.ExpiresAtUtc);
    }

    private async Task<PendingTotpSetupEntity?> GetEntityAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await _dbContext.PendingTotpSetups
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
