using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Authentication.Common;

public sealed record PendingLoginChallenge(
    Guid Id,
    UserId UserId,
    DateTime ExpiresAtUtc,
    int Attempts,
    bool Consumed
);