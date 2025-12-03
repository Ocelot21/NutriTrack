using NutriTrack.Contracts.Meals;
using NutriTrack.Contracts.UserExerciseLogs;

namespace NutriTrack.Contracts.Me;

public sealed record DailyOverviewResponse(
    IReadOnlyList<MealResponse> Meals,
    IReadOnlyList<UserExerciseLogResponse> Exercises,
    DailyNutritionTargets Targets,
    DailyNutritionSnapshot Snapshot
);

public sealed record DailyNutritionTargets(
    int Calories,
    decimal ProteinGrams,
    decimal FatGrams,
    decimal CarbohydrateGrams);

public sealed record DailyNutritionSnapshot(
    int ConsumedCalories,
    int BurnedCalories,
    int NetCalories,
    int RemainingCalories,
    decimal ConsumedProteinGrams,
    decimal ConsumedFatGrams,
    decimal ConsumedCarbohydrateGrams
);