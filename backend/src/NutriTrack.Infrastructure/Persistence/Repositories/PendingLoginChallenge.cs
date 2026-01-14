using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.TwoFactor;

public sealed class PendingLoginChallengeRepository : IPendingLoginChallengeRepository
{
    private readonly AppDbContext _dbContext;

    public PendingLoginChallengeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PendingLoginChallenge?> GetAsync(Guid challengeId, CancellationToken ct = default)
    {
        var e = await _dbContext.Set<PendingLoginChallengeEntity>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == challengeId, ct);

        return e is null
            ? null
            : new PendingLoginChallenge(e.Id, e.UserId, e.ExpiresAtUtc, e.Attempts, e.Consumed);
    }

    public async Task AddAsync(PendingLoginChallenge challenge, CancellationToken ct = default)
    {
        var entity = new PendingLoginChallengeEntity(challenge.UserId, challenge.ExpiresAtUtc);

        if (challenge.Id != Guid.Empty)
        {
            typeof(PendingLoginChallengeEntity).GetProperty(nameof(PendingLoginChallengeEntity.Id))!
                .SetValue(entity, challenge.Id);
        }

        await _dbContext.Set<PendingLoginChallengeEntity>().AddAsync(entity, ct);
    }

    public async Task IncrementAttemptsAsync(Guid challengeId, CancellationToken ct = default)
    {
        var e = await _dbContext.Set<PendingLoginChallengeEntity>()
            .SingleOrDefaultAsync(x => x.Id == challengeId, ct);

        if (e is null) return;

        e.IncrementAttempts();
    }

    public async Task ConsumeAsync(Guid challengeId, CancellationToken ct = default)
    {
        var e = await _dbContext.Set<PendingLoginChallengeEntity>()
            .SingleOrDefaultAsync(x => x.Id == challengeId, ct);

        if (e is null) return;

        e.Consume();
    }

    public async Task DeleteAsync(Guid challengeId, CancellationToken ct = default)
    {
        var e = await _dbContext.Set<PendingLoginChallengeEntity>()
            .SingleOrDefaultAsync(x => x.Id == challengeId, ct);

        if (e is null) return;

        _dbContext.Set<PendingLoginChallengeEntity>().Remove(e);
    }

    public Task DeleteExpiredAsync(DateTime utcNow, CancellationToken ct = default)
        => _dbContext.Set<PendingLoginChallengeEntity>()
            .Where(x => x.ExpiresAtUtc <= utcNow || x.Consumed)
            .ExecuteDeleteAsync(ct);
}
