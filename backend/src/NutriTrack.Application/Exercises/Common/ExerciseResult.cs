using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Common;

public sealed record ExerciseResult(
    ExerciseId Id,
    string Name,
    ExerciseCategory Category,
    decimal DefaultCaloriesPerMinute,
    string? Description,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted
);
