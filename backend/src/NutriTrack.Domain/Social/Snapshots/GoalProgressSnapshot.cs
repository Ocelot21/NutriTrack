using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Social.Snapshots;

public sealed class GoalProgressSnapshot : Entity<GoalProgressSnapshotId>
{
    private readonly List<GoalProgressSnapshotPoint> _points = new();

    private GoalProgressSnapshot() { } // EF

    private GoalProgressSnapshot(
        GoalProgressSnapshotId id,
        UserGoalId userGoalId,
        NutritionGoal goalType,
        DateOnly goalStartDate,
        DateOnly goalTargetDate,
        decimal startWeightKg,
        decimal targetWeightKg,
        decimal currentWeightKg,
        DateOnly snapshotDate) : base(id)
    {
        UserGoalId = userGoalId;
        GoalType = goalType;
        GoalStartDate = goalStartDate;
        GoalTargetDate = goalTargetDate;

        StartWeightKg = startWeightKg;
        TargetWeightKg = targetWeightKg;
        CurrentWeightKg = currentWeightKg;

        SnapshotDate = snapshotDate;
    }

    public UserGoalId UserGoalId { get; private set; }
    public UserGoal UserGoal { get; private set; } = null!;

    public NutritionGoal GoalType { get; private set; }

    public DateOnly GoalStartDate { get; private set; }
    public DateOnly GoalTargetDate { get; private set; }
    public DateOnly SnapshotDate { get; private set; }

    public decimal StartWeightKg { get; private set; }
    public decimal CurrentWeightKg { get; private set; }
    public decimal TargetWeightKg { get; private set; }

    public IReadOnlyList<GoalProgressSnapshotPoint> Points => _points;

    public static GoalProgressSnapshot Create(
        UserGoal goal,
        decimal currentWeightKg,
        DateOnly snapshotDate,
        IEnumerable<(DateOnly date, decimal weightKg)> points)
    {
        var id = new GoalProgressSnapshotId(Guid.NewGuid());

        var snapshot = new GoalProgressSnapshot(
            id,
            goal.Id,
            goal.Type,
            goal.StartDate,
            goal.TargetDate,
            goal.StartWeightKg,
            goal.TargetWeightKg,
            currentWeightKg,
            snapshotDate);

        foreach (var (date, weightKg) in points.OrderBy(x => x.date))
        {
            snapshot._points.Add(GoalProgressSnapshotPoint.Create(date, weightKg));
        }

        return snapshot;
    }
}
