namespace NutriTrack.Contracts.Exercises;

public sealed record ExerciseResponse(
    Guid Id,
    string Name,
    string Category,
    decimal DefaultCaloriesPerMinute,
    string? Description,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted
);
