using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.TwoFactor;

public sealed class PendingTotpSetupEntity
{
    public UserId UserId { get; private set; }
    public User? User { get; private set; }
    public string SecretProtected { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }

    private PendingTotpSetupEntity() { }

    public PendingTotpSetupEntity(UserId userId, string secretProtected, DateTime expiresAtUtc)
    {
        UserId = userId;
        SecretProtected = secretProtected;
        ExpiresAtUtc = expiresAtUtc;
    }
}
