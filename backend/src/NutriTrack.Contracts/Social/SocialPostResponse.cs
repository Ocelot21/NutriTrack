using NutriTrack.Contracts.UserAchievements;

namespace NutriTrack.Contracts.Social;

public sealed record SocialPostResponse(
    Guid Id,
    SocialPostAuthorResponse Author,
    int Type,
    int Visibility,
    DateTime LocalTime,
    string? Text,
    UserAchievementResponse? UserAchievement,
    DailyOverviewSnapshotResponse? DailyOverviewSnapshot,
    GoalProgressSnapshotResponse? GoalProgressSnapshot
);

public sealed record SocialPostAuthorResponse(
    Guid Id,
    string Username,
    string? AvatarUrl
);

public sealed record DailyOverviewSnapshotResponse(
    Guid Id,
    DateOnly Date,
    int TargetCalories,
    decimal TargetProteinGrams,
    decimal TargetFatGrams,
    decimal TargetCarbohydrateGrams,
    int ConsumedCalories,
    int BurnedCalories,
    int NetCalories,
    int RemainingCalories,
    decimal ConsumedProteinGrams,
    decimal ConsumedFatGrams,
    decimal ConsumedCarbohydrateGrams,
    int MealCount,
    int ExerciseCount
);

public sealed record GoalProgressSnapshotResponse(
    Guid Id,
    Guid UserGoalId,
    string GoalType,
    DateOnly GoalStartDate,
    DateOnly GoalTargetDate,
    DateOnly SnapshotDate,
    decimal StartWeightKg,
    decimal CurrentWeightKg,
    decimal TargetWeightKg,
    IReadOnlyList<GoalProgressSnapshotPointResponse> Points
);

public sealed record GoalProgressSnapshotPointResponse(
    DateOnly Date,
    decimal WeightKg
);