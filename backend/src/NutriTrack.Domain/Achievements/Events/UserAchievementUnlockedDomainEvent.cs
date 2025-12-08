using NutriTrack.Domain.Common.Events;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Achievements.Events;

public sealed record UserAchievementUnlockedDomainEvent(
    UserAchievementId UserAchievementId,
    UserId UserId,
    AchievementId AchievementId) : IDomainEvent;

