using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Persistence.Seed.Authorization;

public static class AuthorizationProfiles
{
    public static readonly string[] UserBasic =
    {
        PermissionKeys.Meals.Read_Own,
        PermissionKeys.Meals.Create,
        PermissionKeys.Meals.Update_Own,
        PermissionKeys.UserExerciseLogs.Read_Own,
        PermissionKeys.UserExerciseLogs.Create,
        PermissionKeys.UserExerciseLogs.Update_Own
    };

    public static readonly string[] AdminAll = PermissionKeys.All;
}