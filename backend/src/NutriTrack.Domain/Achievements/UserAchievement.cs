using NutriTrack.Domain.Achievements.Events;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Achievements;

public sealed class UserAchievement : AggregateRoot<UserAchievementId>
{
    public UserId UserId { get; private set; }
    public User User { get; private set; } = null!;
    public AchievementId AchievementId { get; private set; }
    public Achievement Achievement { get; private set; } = null!;
    public DateOnly LocalDateEarned { get; private set; }


    private UserAchievement() : base() { }

    private UserAchievement(
        UserAchievementId id,
        UserId userId,
        AchievementId achievementId,
        DateOnly localDateEarned) : base(id)
    {
        UserId = userId;
        AchievementId = achievementId;
        LocalDateEarned = localDateEarned;
    }

    public static UserAchievement Create(
        UserId userId,
        AchievementId achievementId,
        DateOnly localDateEarned)
    {
        var id = new UserAchievementId(Guid.NewGuid());

        var userAchievement = new UserAchievement(
            id,
            userId,
            achievementId,
            localDateEarned);

        userAchievement.RaiseDomainEvent(new UserAchievementUnlockedDomainEvent(
            id,
            userId,
            achievementId));

        return userAchievement;
    }
}
