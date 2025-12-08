using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

internal static partial class ConversionsExtensions
{
    public static PropertyBuilder<UserGoalId> HasUserGoalIdConversion(this PropertyBuilder<UserGoalId> propertyBuilder)
        => Configurations.Conversions.HasIdConversion(propertyBuilder, id => id.Value, value => new UserGoalId(value));

    public static PropertyBuilder<ActivityLevelHistoryId> HasActivityLevelHistoryIdConversion(this PropertyBuilder<ActivityLevelHistoryId> propertyBuilder)
        => Configurations.Conversions.HasIdConversion(propertyBuilder, id => id.Value, value => new ActivityLevelHistoryId(value));

    public static PropertyBuilder<WeightHistoryEntryId> HasWeightHistoryIdConversion(this PropertyBuilder<WeightHistoryEntryId> propertyBuilder)
        => Configurations.Conversions.HasIdConversion(propertyBuilder, id => id.Value, value => new WeightHistoryEntryId(value));
}
