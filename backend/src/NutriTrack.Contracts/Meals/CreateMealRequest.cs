namespace NutriTrack.Contracts.Meals;

public sealed record CreateMealRequest(
    string Name,
    DateTimeOffset OccurredAtLocal,
    string? Description
);
