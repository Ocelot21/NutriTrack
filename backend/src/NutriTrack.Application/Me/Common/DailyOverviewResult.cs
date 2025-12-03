using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Application.UserExercises.Common;

namespace NutriTrack.Application.Me.Common;

public sealed record DailyOverviewResult(
    IReadOnlyList<MealResult> Meals,
    IReadOnlyList<UserExerciseLogResult> Exercises,
    DailyNutritionTargets Targets,
    DailyNutritionSnapshot Snapshot);

public sealed record DailyNutritionSnapshot(
    int ConsumedCalories,
    int BurnedCalories,
    int NetCalories,
    int RemainingCalories,
    decimal ConsumedProteinGrams,
    decimal ConsumedFatGrams,
    decimal ConsumedCarbohydrateGrams
);