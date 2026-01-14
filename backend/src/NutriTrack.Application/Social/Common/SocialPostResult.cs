using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Common;

public sealed record SocialPostResult(
    SocialPostId Id,
    SocialPostAuthorResult Author,
    PostType Type,
    PostVisibility Visibility,
    DateTime LocalTime,
    string? Text,
    UserAchievement? UserAchievement,
    DailyOverviewSnapshot? DailyOverviewSnapshot,
    GoalProgressSnapshot? GoalProgressSnapshot
);

public sealed record SocialPostAuthorResult(
    UserId Id,
    Username Username,
    string? AvatarUrl
);
