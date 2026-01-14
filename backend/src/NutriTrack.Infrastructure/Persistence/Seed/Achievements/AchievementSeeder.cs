using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infrastructure.Persistence.Seed.Achievements;

public class AchievementSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public AchievementSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int Order => 50;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var achievements = new[]
        {
        // GOALS
        Achievement.Create(
            key: "GOALS_COMPLETED_1",
            title: "First goal completed",
            description: "You successfully completed your first goal.",
            points: 10,
            category: AchievementCategory.Goals,
            iconName: "flag"
        ),
        Achievement.Create(
            key: "GOALS_COMPLETED_5",
            title: "5 goals completed",
            description: "You have completed 5 goals in total.",
            points: 30,
            category: AchievementCategory.Goals,
            iconName: "flag_circle"
        ),
        Achievement.Create(
            key: "GOALS_COMPLETED_10",
            title: "10 goals completed",
            description: "You have completed 10 goals in total.",
            points: 60,
            category: AchievementCategory.Goals,
            iconName: "military_tech"
        ),
        Achievement.Create(
            key: "GOALS_COMPLETED_20",
            title: "20 goals completed",
            description: "You have completed 20 goals in total.",
            points: 120,
            category: AchievementCategory.Goals,
            iconName: "emoji_events"
        ),

        // EXERCISES
        Achievement.Create(
            key: "EXERCISE_FIRST_LOG",
            title: "First workout logged",
            description: "You logged your first exercise.",
            points: 5,
            category: AchievementCategory.Exercises,
            iconName: "fitness_center"
        ),
        Achievement.Create(
            key: "EXERCISE_STREAK_3_DAYS",
            title: "3-day exercise streak",
            description: "You logged exercise for 3 days in a row.",
            points: 15,
            category: AchievementCategory.Exercises,
            iconName: "directions_run"
        ),
        Achievement.Create(
            key: "EXERCISE_STREAK_7_DAYS",
            title: "7-day exercise streak",
            description: "You logged exercise for 7 days in a row.",
            points: 35,
            category: AchievementCategory.Exercises,
            iconName: "self_improvement"
        ),
        Achievement.Create(
            key: "EXERCISE_STREAK_30_DAYS",
            title: "30-day exercise streak",
            description: "You logged exercise for 30 days in a row.",
            points: 120,
            category: AchievementCategory.Exercises,
            iconName: "sports_gymnastics"
        ),

        // MEALS
        Achievement.Create(
            key: "FIRST_MEAL_ITEM",
            title: "First meal logged",
            description: "You logged your first meal item.",
            points: 5,
            category: AchievementCategory.Meals,
            iconName: "restaurant"
        ),
        Achievement.Create(
            key: "MEAL_ITEM_STREAK_3_DAYS",
            title: "3-day meal streak",
            description: "You logged meals for 3 days in a row.",
            points: 15,
            category: AchievementCategory.Meals,
            iconName: "restaurant_menu"
        ),
        Achievement.Create(
            key: "MEAL_ITEM_STREAK_7_DAYS",
            title: "7-day meal streak",
            description: "You logged meals for 7 days in a row.",
            points: 35,
            category: AchievementCategory.Meals,
            iconName: "lunch_dining"
        ),
        Achievement.Create(
            key: "MEAL_ITEM_STREAK_30_DAYS",
            title: "30-day meal streak",
            description: "You logged meals for 30 days in a row.",
            points: 120,
            category: AchievementCategory.Meals,
            iconName: "fastfood"
        ),

        // CALORIES
        Achievement.Create(
            key: "CALORIES_BURNED_1000",
            title: "1,000 kcal burned",
            description: "You have burned a total of 1,000 calories.",
            points: 10,
            category: AchievementCategory.Calories,
            iconName: "local_fire_department"
        ),
        Achievement.Create(
            key: "CALORIES_BURNED_10000",
            title: "10,000 kcal burned",
            description: "You have burned a total of 10,000 calories.",
            points: 50,
            category: AchievementCategory.Calories,
            iconName: "whatshot"
        ),
        Achievement.Create(
            key: "CALORIES_BURNED_50000",
            title: "50,000 kcal burned",
            description: "You have burned a total of 50,000 calories.",
            points: 150,
            category: AchievementCategory.Calories,
            iconName: "local_fire_department"
        ),
    };

        var existingKeys = await _dbContext.Achievements
            .Select(a => a.Key)
            .ToListAsync(cancellationToken);

        var toInsert = achievements
            .Where(a => !existingKeys.Contains(a.Key))
            .ToList();

        if (toInsert.Any())
        {
            await _dbContext.Achievements.AddRangeAsync(toInsert, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }

}