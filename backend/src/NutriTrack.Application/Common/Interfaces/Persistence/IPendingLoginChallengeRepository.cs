using NutriTrack.Application.Authentication.Common;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IPendingLoginChallengeRepository
{
    Task<PendingLoginChallenge?> GetAsync(Guid challengeId, CancellationToken ct = default);

    Task AddAsync(PendingLoginChallenge challenge, CancellationToken ct = default);

    Task IncrementAttemptsAsync(Guid challengeId, CancellationToken ct = default);

    Task ConsumeAsync(Guid challengeId, CancellationToken ct = default);

    Task DeleteAsync(Guid challengeId, CancellationToken ct = default);

    Task DeleteExpiredAsync(DateTime utcNow, CancellationToken ct = default);
}
