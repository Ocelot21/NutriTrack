namespace NutriTrack.Application.Reports.Common;

public sealed record UserActivityReportData(
    int MealsCount,
    int TotalCaloriesConsumed,
    decimal TotalProteinGrams,
    decimal TotalCarbsGrams,
    decimal TotalFatGrams,
    int ExerciseCount,
    decimal TotalMinutesExercised,
    decimal TotalCaloriesBurned
);
