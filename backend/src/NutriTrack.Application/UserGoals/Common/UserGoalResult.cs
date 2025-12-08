using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Common;

public sealed record UserGoalResult(
    UserGoalId Id,
    UserId UserId,
    NutritionGoal Type,
    UserGoalStatus Status,
    DateOnly StartDate,
    DateOnly TargetDate,
    decimal StartWeightKg,
    decimal TargetWeightKg,
    DateTime? CompletedAtUtc,
    DateTime? FailedAtUtc
);
