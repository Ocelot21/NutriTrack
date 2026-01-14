using NutriTrack.Application.Social.Common;

namespace NutriTrack.Application.Social.Queries.GetSocialProfile;

public sealed record SocialProfileResult(
    Guid UserId,
    string Username,
    string? AvatarUrl,
    int TotalPoints,
    IReadOnlyList<SocialPostResult> Posts,
    IReadOnlyList<SocialProfileAchievementResult> Achievements);

public sealed record SocialProfileAchievementResult(
    string Name,
    string? Icon);
