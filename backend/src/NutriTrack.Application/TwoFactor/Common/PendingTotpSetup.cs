using NutriTrack.Domain.Users;

namespace NutriTrack.Application.TwoFactor.Common;

public sealed record PendingTotpSetup(
    UserId UserId,
    string SecretProtected,
    DateTime ExpiresAtUtc
);