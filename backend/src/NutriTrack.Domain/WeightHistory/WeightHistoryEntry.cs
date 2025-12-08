using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.WeightHistory;

public sealed class WeightHistoryEntry : AggregateRoot<WeightHistoryEntryId>
{
    public UserId UserId { get; private set; }

    public User User { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    public decimal WeightKg { get; private set; }

    private WeightHistoryEntry() : base()
    {
    }

    private WeightHistoryEntry(
        WeightHistoryEntryId id,
        UserId userId,
        DateOnly date,
        decimal weightKg) : base(id)
    {
        UserId = userId;
        Date = date;
        WeightKg = weightKg;
    }

    public static WeightHistoryEntry Create(
        UserId userId,
        DateOnly date,
        decimal weightKg,
        DateTime utcNow)
    {
        if (weightKg <= 0)
        {
            throw new ArgumentException("Weight must be positive.", nameof(weightKg));
        }

        var id = new WeightHistoryEntryId(Guid.NewGuid());

        var entry = new WeightHistoryEntry(
            id,
            userId,
            date,
            weightKg);

        entry.SetCreated(utcNow, userId);

        return entry;
    }
}
