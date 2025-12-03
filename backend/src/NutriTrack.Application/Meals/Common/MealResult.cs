using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Common;

public sealed record MealResult(
    MealId Id,
    UserId UserId,
    string Name,
    string? Description,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    int TotalCalories,
    decimal TotalProtein,
    decimal TotalCarbohydrates,
    decimal TotalFats,
    IReadOnlyList<MealItem> Items
);
