namespace NutriTrack.Application.Common.Models;

public sealed record DailyNutritionTargets(
    int Calories,
    decimal ProteinGrams,
    decimal FatGrams,
    decimal CarbohydrateGrams);