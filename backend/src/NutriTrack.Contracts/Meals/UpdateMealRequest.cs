namespace NutriTrack.Contracts.Meals;

public sealed record UpdateMealRequest(
    string? Name,
    string? Description,
    DateTimeOffset? OccurredAtLocal
);
