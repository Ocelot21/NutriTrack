namespace NutriTrack.Contracts.UserExerciseLogs;

public sealed record UserExerciseLogResponse(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    string Category,
    decimal DurationMinutes,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    decimal TotalCalories,
    string? Notes
);
