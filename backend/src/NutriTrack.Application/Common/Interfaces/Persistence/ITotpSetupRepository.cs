using NutriTrack.Application.TwoFactor.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IPendingTotpSetupRepository
{
    Task<PendingTotpSetup?> GetAsync(UserId userId, CancellationToken cancellationToken);
    Task UpsertAsync(PendingTotpSetup setup, CancellationToken cancellationToken);
    Task DeleteAsync(UserId userId, CancellationToken cancellationToken);
}