namespace NutriTrack.Domain.Common;


public static class DomainConstraints
{
    public static class Users
    {
        public const int MaxNameLength = 100;
        public const int MinUsernameLength = 3;
        public const int MaxUsernameLength = 32;
        public const int MaxEmailLength = 255;
    }

    public static class Groceries
    {
        public const int MaxNameLength = 200;
        public const int MaxBarcodeLength = 64;
        public const int MaxCaloriesPer100 = 5000;
        public const int MaxMacroValuePer100 = 1000;
    }

    public static class Exercises
    {
        public const int MaxNameLength = 150;
        public const int MaxDescriptionLength = 1000;
        public const decimal MaxDefaultCaloriesPerMinute = 2000m;
    }

    public static class Authorization
    {
        public const int MaxPermissionKeyLength = 100;
        public const int MaxPermissionDescriptionLength = 500;
        public const int MaxRoleNameLength = 100;
        public const int MaxRoleDescriptionLength = 500;
    }

    public static class Meals
    {
        public const int MaxMealNameLength = 120;
        public const int MaxMealDescriptionLength = 500;
        public const int MaxMealItemNotesLength = 1000;
    }

    public static class UserExerciseLogs
    {
        public const int MaxNotesLength = 1000;
        public const int MaxExerciseSnapshotNameLength = 150;
    }

    public static class Countries
    {
        public const int MaxCountryNameLength = 100;
    }
}
