using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Countries;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

internal static class Conversions
{
    public static PropertyBuilder<TId> HasGuidConversion<TId>(
            this PropertyBuilder<TId> propertyBuilder,
            Func<TId, Guid> fromId,
            Func<Guid, TId> toId)
    {
        propertyBuilder.HasConversion(
            id => fromId(id),
            value => toId(value)
        );

        propertyBuilder.ValueGeneratedNever();
        return propertyBuilder;
    }

    public static PropertyBuilder<TId> HasOptionalGuidConversion<TId>(
            this PropertyBuilder<TId> propertyBuilder,
            Func<TId, Guid?> fromId,
            Func<Guid?, TId> toId)
    {
        propertyBuilder.HasConversion(
            id => fromId(id),
            value => toId(value)
        );

        propertyBuilder.ValueGeneratedNever();
        return propertyBuilder;
    }

    public static PropertyBuilder<TId> HasStringIdConversion<TId>(
        this PropertyBuilder<TId> propertyBuilder,
        Func<TId, string> fromId,
        Func<string, TId> toId)
    {
        propertyBuilder.HasConversion(
            id => fromId(id),
            value => toId(value)
        );

        propertyBuilder.ValueGeneratedNever();
        return propertyBuilder;
    }

    public static PropertyBuilder<TId> HasOptionalStringIdConversion<TId>(
        this PropertyBuilder<TId> propertyBuilder,
        Func<TId, string?> fromId,
        Func<string?, TId> toId)
    {
        propertyBuilder.HasConversion(
            id => fromId(id),
            value => toId(value)
        );

        propertyBuilder.ValueGeneratedNever();
        return propertyBuilder;
    }

    public static PropertyBuilder<UserId> HasUserIdConversion(
        this PropertyBuilder<UserId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new UserId(value));

    public static PropertyBuilder<RoleId> HasRoleIdConversion(
        this PropertyBuilder<RoleId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new RoleId(value));

    public static PropertyBuilder<PermissionId> HasPermissionIdConversion(
        this PropertyBuilder<PermissionId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new PermissionId(value));

    public static PropertyBuilder<GroceryId> HasGroceryIdConversion(
        this PropertyBuilder<GroceryId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new GroceryId(value));

    public static PropertyBuilder<MealId> HasMealIdConversion(
        this PropertyBuilder<MealId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new MealId(value));

    public static PropertyBuilder<MealItemId> HasMealItemIdConversion(
        this PropertyBuilder<MealItemId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new MealItemId(value));

    public static PropertyBuilder<ExerciseId> HasExerciseIdConversion(
        this PropertyBuilder<ExerciseId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new ExerciseId(value));

    public static PropertyBuilder<UserExerciseLogId> HasUserExerciseLogIdConversion(
        this PropertyBuilder<UserExerciseLogId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new UserExerciseLogId(value));

    public static PropertyBuilder<UserGoalId> HasUserGoalIdConversion(
        this PropertyBuilder<UserGoalId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new UserGoalId(value));

    public static PropertyBuilder<ActivityLevelHistoryId> HasActivityLevelHistoryIdConversion(
        this PropertyBuilder<ActivityLevelHistoryId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new ActivityLevelHistoryId(value));

    public static PropertyBuilder<WeightHistoryEntryId> HasWeightHistoryIdConversion(
        this PropertyBuilder<WeightHistoryEntryId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new WeightHistoryEntryId(value));

    public static PropertyBuilder<AchievementId> HasAchievementIdConversion(
        this PropertyBuilder<AchievementId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new AchievementId(value));

    public static PropertyBuilder<UserAchievementId> HasUserAchievementIdConversion(
        this PropertyBuilder<UserAchievementId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new UserAchievementId(value));

    public static PropertyBuilder<SocialPostId> HasSocialPostIdConversion(
        this PropertyBuilder<SocialPostId> propertyBuilder)
        => HasGuidConversion(propertyBuilder, id => id.Value, value => new SocialPostId(value));

    public static PropertyBuilder<DailyOverviewSnapshotId> HasDailyOverviewSnapshotIdConversion(
        this PropertyBuilder<DailyOverviewSnapshotId> propertyBuilder)
        => HasGuidConversion(
            propertyBuilder,
            id => id.Value,
            value => new DailyOverviewSnapshotId(value));

    public static PropertyBuilder<GoalProgressSnapshotId> HasGoalProgressSnapshotIdConversion(
        this PropertyBuilder<GoalProgressSnapshotId> propertyBuilder)
        => HasGuidConversion(
            propertyBuilder,
            id => id.Value,
            value => new GoalProgressSnapshotId(value));

    public static PropertyBuilder<MealId?> HasOptionalMealIdConversion(
        this PropertyBuilder<MealId?> propertyBuilder)
        => HasOptionalGuidConversion(
            propertyBuilder,
            id => id?.Value,
            value =>
            {
                if (!value.HasValue)
                    return null;

                return new MealId(value.Value);
            });

    public static PropertyBuilder<UserAchievementId?> HasOptionalUserAchievementIdConversion(
        this PropertyBuilder<UserAchievementId?> propertyBuilder)
        => HasOptionalGuidConversion(
            propertyBuilder, 
            id => id?.Value,
            value =>
            {
                if (!value.HasValue)
                    return null;

                return new UserAchievementId(value.Value);
            }
        );

    public static PropertyBuilder<CountryCode> HasCountryCodeConversion(
        this PropertyBuilder<CountryCode> propertyBuilder)
        => propertyBuilder.HasStringIdConversion(
            id => id.Value,
            value => CountryCode.Create(value));

    public static PropertyBuilder<CountryCode?> HasOptionalCountryCodeConversion(
    this PropertyBuilder<CountryCode?> propertyBuilder)
    => propertyBuilder.HasOptionalStringIdConversion(
            id => id?.Value,
            value => CountryCode.CreateOptional(value)
        );

    public static PropertyBuilder<DailyOverviewSnapshotId?> HasOptionalDailyOverviewSnapshotIdConversion(
        this PropertyBuilder<DailyOverviewSnapshotId?> propertyBuilder)
        => propertyBuilder.HasOptionalGuidConversion(
            id => id?.Value,
            value =>
            {
                if (!value.HasValue)
                    return null;

                return new DailyOverviewSnapshotId(value.Value);
            });

    public static PropertyBuilder<GoalProgressSnapshotId?> HasOptionalGoalProgressSnapshotIdConversion(
        this PropertyBuilder<GoalProgressSnapshotId?> propertyBuilder)
        => propertyBuilder.HasOptionalGuidConversion(
            id => id?.Value,
            value =>
            {
                if (!value.HasValue)
                    return null;

                return new GoalProgressSnapshotId(value.Value);
            });
    
    public static PropertyBuilder<ReportRunId> HasReportRunIdConversion(
        this PropertyBuilder<ReportRunId> propertyBuilder)
        => propertyBuilder.HasGuidConversion(id => id.Value, value => new ReportRunId(value));
}