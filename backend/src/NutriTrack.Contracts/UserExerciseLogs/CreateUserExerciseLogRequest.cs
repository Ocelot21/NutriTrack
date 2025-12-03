namespace NutriTrack.Contracts.UserExerciseLogs;

public sealed record CreateUserExerciseLogRequest(
    Guid ExerciseId,
    decimal DurationMinutes,
    DateTimeOffset OccurredAtLocal,
    string? Notes
);
