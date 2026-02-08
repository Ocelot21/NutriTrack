namespace NutriTrack.Contracts.UserExerciseLogs;

public sealed record UpdateUserExerciseLogRequest(
    decimal? DurationMinutes,
    DateTimeOffset? OccurredAtLocal,
    string? Notes
);
