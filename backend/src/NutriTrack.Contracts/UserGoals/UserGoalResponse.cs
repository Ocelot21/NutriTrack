namespace NutriTrack.Contracts.UserGoals;

public record UserGoalResponse
(
    Guid Id,
    Guid UserId,
    int Type,
    int Status,
    DateOnly StartDate,
    DateOnly TargetDate,
    decimal StartWeightKg,
    decimal TargetWeightKg,
    DateTime? CompletedAtUtc,
    DateTime? FailedAtUtc
);