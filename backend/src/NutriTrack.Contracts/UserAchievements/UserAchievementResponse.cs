namespace NutriTrack.Contracts.UserAchievements
{
    public record UserAchievementResponse(
        Guid Id,
        Guid UserId,
        Guid AchievementId,
        string Key,
        string Title,
        string Description,
        int Points,
        string Category,
        string? IconName,
        DateOnly LocalDateEarned
    );
}
