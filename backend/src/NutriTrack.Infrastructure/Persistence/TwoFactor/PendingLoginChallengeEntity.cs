using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.TwoFactor;

public sealed class PendingLoginChallengeEntity
{
    private PendingLoginChallengeEntity() { }

    public PendingLoginChallengeEntity(UserId userId, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ExpiresAtUtc = expiresAtUtc;
        Attempts = 0;
        Consumed = false;
    }

    public Guid Id { get; private set; }

    public UserId UserId { get; private set; } = default!;
    public User? User { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public int Attempts { get; private set; }
    public bool Consumed { get; private set; }

    public void IncrementAttempts() => Attempts++;
    public void Consume() => Consumed = true;
}
