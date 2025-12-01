namespace NutriTrack.Contracts.Meals;

public sealed record CreateMealRequest(
    string Name,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    string? Description
);
