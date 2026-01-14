namespace NutriTrack.Contracts.Exercises;

public sealed record CreateExerciseRequest(
    string Name,
    string Category,
    decimal DefaultCaloriesPerMinute,
    string? Description
);
