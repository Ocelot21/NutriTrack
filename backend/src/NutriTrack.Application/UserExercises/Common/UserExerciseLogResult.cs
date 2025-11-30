using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.UserExercises.Common;

public sealed record UserExerciseLogResult(
    UserExerciseLogId Id,
    UserId UserId,
    ExerciseId ExerciseId,
    string ExerciseName,
    ExerciseCategory Category,
    decimal DurationMinutes,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    decimal TotalCalories,
    string? Notes
);
