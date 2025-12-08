using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserAchievements.Common
{
    public record UserAchievementResult(
        UserAchievementId Id,
        UserId UserId,
        AchievementId AchievementId,
        string Key,
        string Title,
        string Description,
        int Points,
        AchievementCategory Category,
        string? IconName,
        DateOnly LocalDateEarned
    );
}
