namespace NutriTrack.Contracts.UserGoals;

public sealed record CreateUserGoalRequest(
    int Type,
    DateOnly TargetDate,
    decimal TargetWeightKg
);
