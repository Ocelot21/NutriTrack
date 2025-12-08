namespace NutriTrack.Domain.Notifications;

public enum NotificationType
{
    Unknown = 0,

    // Core NutriTrack events
    DailyGoalReached = 1,
    CaloriesExceeded = 2,
    StreakExtended = 3,
    InactivityReminder = 4,
    AchievementUnlocked = 5,
    // Account / security related
    EmailVerified = 10,
    PasswordResetRequested = 11,
    TwoFactorCodeSent = 12,

    // System-level / generic messages
    System = 20,
    Custom = 21
}
