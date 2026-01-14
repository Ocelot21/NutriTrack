namespace NutriTrack.Application.Reports.Common;

public sealed record WeeklyOverviewReportData(
    int MealsCount,
    int TotalCaloriesConsumed,
    decimal TotalProteinGrams,
    decimal TotalCarbsGrams,
    decimal TotalFatGrams,
    int ExerciseCount,
    decimal TotalMinutesExercised,
    decimal TotalCaloriesBurned
);
