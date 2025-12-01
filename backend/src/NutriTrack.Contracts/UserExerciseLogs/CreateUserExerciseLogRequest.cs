namespace NutriTrack.Contracts.UserExerciseLogs;

public sealed record CreateUserExerciseLogRequest(
    Guid ExerciseId,
    decimal DurationMinutes,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    string? Notes
);
