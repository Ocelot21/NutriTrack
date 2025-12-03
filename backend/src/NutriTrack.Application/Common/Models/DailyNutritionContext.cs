using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Models;

public sealed record DailyNutritionContext(
    Gender Gender,
    int AgeYears,
    decimal HeightCm,
    decimal WeightKg,
    ActivityLevel ActivityLevel,
    NutritionGoal Goal);
