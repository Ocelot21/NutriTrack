using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

internal static class Conversions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(
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


    public static PropertyBuilder<UserId> HasUserIdConversion(this PropertyBuilder<UserId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new UserId(value));

    public static PropertyBuilder<RoleId> HasRoleIdConversion(this PropertyBuilder<RoleId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new RoleId(value));

    public static PropertyBuilder<PermissionId> HasPermissionIdConversion(this PropertyBuilder<PermissionId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new PermissionId(value));

    public static PropertyBuilder<GroceryId> HasGroceryIdConversion(this PropertyBuilder<GroceryId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new GroceryId(value));

    public static PropertyBuilder<MealId> HasMealIdConversion(this PropertyBuilder<MealId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new MealId(value));

    public static PropertyBuilder<MealItemId> HasMealItemIdConversion(this PropertyBuilder<MealItemId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new MealItemId(value));

    public static PropertyBuilder<ExerciseId> HasExerciseIdConversion(this PropertyBuilder<ExerciseId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new ExerciseId(value));

    public static PropertyBuilder<UserExerciseLogId> HasUserExerciseLogIdConversion(this PropertyBuilder<UserExerciseLogId> propertyBuilder)
        => propertyBuilder.HasIdConversion(id => id.Value, value => new UserExerciseLogId(value));
}

