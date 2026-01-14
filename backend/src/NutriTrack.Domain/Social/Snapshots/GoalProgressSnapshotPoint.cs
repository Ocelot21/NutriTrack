namespace NutriTrack.Domain.Social.Snapshots;

public sealed class GoalProgressSnapshotPoint
{
    private GoalProgressSnapshotPoint() { } // EF

    private GoalProgressSnapshotPoint(DateOnly date, decimal weightKg)
    {
        Date = date;
        WeightKg = weightKg;
    }

    public DateOnly Date { get; private set; }
    public decimal WeightKg { get; private set; }

    public static GoalProgressSnapshotPoint Create(DateOnly date, decimal weightKg)
    {
        if (weightKg <= 0) throw new ArgumentException("Weight must be positive.", nameof(weightKg));
        return new GoalProgressSnapshotPoint(date, weightKg);
    }
}
