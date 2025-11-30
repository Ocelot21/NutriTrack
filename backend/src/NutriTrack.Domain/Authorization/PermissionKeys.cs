namespace NutriTrack.Domain.Authorization;

public static class PermissionKeys
{
    public static class Groceries
    {
        public const string Read = "Groceries.Read";
        public const string Create = "Groceries.Create";
        public const string Update = "Groceries.Update";
        public const string Delete = "Groceries.Delete";
        public const string UploadImage = "Groceries.UploadImage";
    }

    public static class Exercises
    {
        public const string Read = "Exercises.Read";
        public const string Create = "Exercises.Create";
        public const string Update = "Exercises.Update";
        public const string Delete = "Exercises.Delete";
        public const string UploadImage = "Exercises.UploadImage";
    }

    public static class Meals
    {
        public const string Read_Own = "Meals.Read.Own";
        public const string Read_All = "Meals.Read.All";
        public const string Create = "Meals.Create";
        public const string Update_Own = "Meals.Update.Own";
    }

    public static class UserExerciseLogs
    {
        public const string Read_Own = "UserExerciseLogs.Read.Own";
        public const string Read_All = "UserExerciseLogs.Read.All";
        public const string Create = "UserExerciseLogs.Create";
        public const string Update_Own = "UserExerciseLogs.Update.Own";
    }

    public static class Users
    {
        public const string Read = "Users.Read";
        public const string Manage = "Users.Manage";
        public const string SetRole = "Users.SetRole";
    }

    public static class Roles
    {
        public const string Read = "Roles.Read";
        public const string Create = "Roles.Create";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
    }

    public static class RolePermissions
    {
        public const string Manage = "RolePermissions.Manage";
    }

    public static readonly string[] All =
    [
        // Groceries
        Groceries.Read, Groceries.Create, Groceries.Update, Groceries.Delete, Groceries.UploadImage,
        //Exercises
        Exercises.Read, Exercises.Create, Exercises.Update, Exercises.Delete, Exercises.UploadImage,
        // Meals
        Meals.Read_Own, Meals.Read_All, Meals.Create, Meals.Update_Own,
        //ExerciseLogs
        UserExerciseLogs.Read_Own, UserExerciseLogs.Read_All, UserExerciseLogs.Create, UserExerciseLogs.Update_Own,
        // Users
        Users.Read, Users.Manage, Users.SetRole,
        // Roles
        Roles.Read, Roles.Create, Roles.Update, Roles.Delete,
        // RolePermissions
        RolePermissions.Manage
    ];
}