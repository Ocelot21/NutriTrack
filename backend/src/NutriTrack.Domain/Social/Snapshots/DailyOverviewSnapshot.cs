using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Social.Snapshots;

public sealed class DailyOverviewSnapshot : Entity<DailyOverviewSnapshotId>
{
    private DailyOverviewSnapshot() { } // EF

    public DailyOverviewSnapshot(
        DailyOverviewSnapshotId id,
        DateOnly date,
        int targetCalories,
        decimal targetProteinGrams,
        decimal targetFatGrams,
        decimal targetCarbohydrateGrams,
        int consumedCalories,
        int burnedCalories,
        int netCalories,
        int remainingCalories,
        decimal consumedProteinGrams,
        decimal consumedFatGrams,
        decimal consumedCarbohydrateGrams,
        int mealCount,
        int exerciseCount)
        : base(id)
    {
        Date = date;

        TargetCalories = targetCalories;
        TargetProteinGrams = targetProteinGrams;
        TargetFatGrams = targetFatGrams;
        TargetCarbohydrateGrams = targetCarbohydrateGrams;

        ConsumedCalories = consumedCalories;
        BurnedCalories = burnedCalories;
        NetCalories = netCalories;
        RemainingCalories = remainingCalories;

        ConsumedProteinGrams = consumedProteinGrams;
        ConsumedFatGrams = consumedFatGrams;
        ConsumedCarbohydrateGrams = consumedCarbohydrateGrams;

        MealCount = mealCount;
        ExerciseCount = exerciseCount;
    }

    public DateOnly Date { get; private set; }

    public int TargetCalories { get; private set; }
    public decimal TargetProteinGrams { get; private set; }
    public decimal TargetFatGrams { get; private set; }
    public decimal TargetCarbohydrateGrams { get; private set; }

    public int ConsumedCalories { get; private set; }
    public int BurnedCalories { get; private set; }
    public int NetCalories { get; private set; }
    public int RemainingCalories { get; private set; }

    public decimal ConsumedProteinGrams { get; private set; }
    public decimal ConsumedFatGrams { get; private set; }
    public decimal ConsumedCarbohydrateGrams { get; private set; }

    public int MealCount { get; private set; }
    public int ExerciseCount { get; private set; }
}
