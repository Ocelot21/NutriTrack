namespace NutriTrack.Contracts.Exercises;

public sealed record UpdateExerciseRequest(
    string? Name,
    string? Category,
    decimal? DefaultCaloriesPerMinute,
    string? Description,
    string? ImageUrl,
    bool? IsApproved,
    bool? IsDeleted
);
