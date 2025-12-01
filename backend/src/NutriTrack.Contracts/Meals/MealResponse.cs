namespace NutriTrack.Contracts.Meals;

public sealed record MealResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    int TotalCalories,
    decimal TotalProtein,
    decimal TotalCarbohydrates,
    decimal TotalFats,
    IReadOnlyList<MealItemResponse> Items
);
