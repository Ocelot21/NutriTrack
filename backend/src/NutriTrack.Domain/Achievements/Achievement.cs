using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Achievements;

public sealed class Achievement : AggregateRoot<AchievementId>
{
    public string Key { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int Points { get; private set; }
    public AchievementCategory Category { get; private set; }

    public string? IconName { get; private set; }

    private Achievement() : base() { }

    private Achievement(
        AchievementId id,
        string key,
        string title,
        string description,
        int points,
        AchievementCategory category,
        string? iconName) : base(id)
    {
        Key = key;
        Title = title;
        Description = description;
        Points = points;
        Category = category;
        IconName = iconName;
    }

    public static Achievement Create(
        string key,
        string title,
        string description,
        int points,
        AchievementCategory category,
        string? iconName)
    {
        return new Achievement(
            new AchievementId(Guid.NewGuid()),
            key,
            title,
            description,
            points,
            category,
            iconName);
    }
}

public enum AchievementCategory
{
    Goals = 1,
    Exercises = 2,
    Meals = 3,
    Calories = 4,
}