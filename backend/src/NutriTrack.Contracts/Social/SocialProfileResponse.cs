namespace NutriTrack.Contracts.Social;

public sealed record SocialProfileResponse(
    Guid UserId,
    string Username,
    string? AvatarUrl,
    int TotalPoints,
    IReadOnlyList<SocialPostResponse> Posts,
    IReadOnlyList<SocialProfileAchievementResponse> Achievements);

public sealed record SocialProfileAchievementResponse(
    string Name,
    string? Icon);
