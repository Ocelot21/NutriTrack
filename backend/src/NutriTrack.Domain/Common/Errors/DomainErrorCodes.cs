namespace NutriTrack.Domain.Common.Errors;

public static class DomainErrorCodes
{
    public static class Authentication
    {
        public const string InvalidCredentials = "Authentication.InvalidCredentials";
        public const string EmailAlreadyInUse = "Authentication.EmailAlreadyInUse";
        public const string UsernameAlreadyExists = "Authentication.UsernameAlreadyExists";
    }

    public static class Users
    {
        public const string InvalidFirstName = "Users.InvalidFirstName";
        public const string InvalidLastName = "Users.InvalidLastName";
        public const string InvalidUsername = "Users.InvalidUsername";
        public const string InvalidEmail = "Users.InvalidEmail";
        public const string InvalidPasswordHash = "Users.InvalidPasswordHash";
        public const string InvalidTimeZone = "Users.InvalidTimeZone";
        public const string InvalidRole = "Users.InvalidRole";

        public const string InvalidCountry = "Users.InvalidCountry";
        public const string InvalidBirthdate = "Users.InvalidBirthdate";
        public const string InvalidHeight = "Users.InvalidHeight";
        public const string InvalidWeight = "Users.InvalidWeight";
        public const string InvalidGender = "Users.InvalidGender";
        public const string InvalidActivityLevel = "Users.InvalidActivityLevel";

        public const string NotFound = "Users.NotFound";
    }

    public static class Groceries
    {
        public const string InvalidName = "Groceries.InvalidName";
        public const string InvalidCategory = "Groceries.InvalidCategory";
        public const string InvalidBarcode = "Groceries.InvalidBarcode";
        public const string InvalidMacros = "Groceries.InvalidMacros";
        public const string InvalidCalories = "Groceries.InvalidCalories";
        public const string NotFound = "Groceries.NotFound";
        public const string InvalidUnitOfMeasure = "Groceries.InvalidUnitOfMeasure";
    }

    public static class Exercises
    {
        public const string InvalidName = "Exercises.InvalidName";
        public const string InvalidDescription = "Exercises.InvalidDescription";
        public const string InvalidCategory = "Exercises.InvalidCategory";
        public const string InvalidCaloriesPerMinute = "Exercises.InvalidCaloriesPerMinute";
        public const string NotFound = "Exercises.NotFound";
    }

    public static class Authorization
    {
        public const string InvalidPermissionKey = "Authorization.InvalidPermissionKey";
        public const string InvalidPermissionDescription = "Authorization.InvalidPermissionDescription";

        public const string InvalidRoleName = "Authorization.InvalidRoleName";
        public const string InvalidRoleDescription = "Authorization.InvalidRoleDescription";

        public const string RoleNotFound = "Authorization.RoleNotFound";
        public const string PermissionNotFound = "Authorization.PermissionNotFound";
    }

    public static class Meals
    {
        public const string InvalidUser = "Meals.InvalidUser";
        public const string InvalidName = "Meals.InvalidName";
        public const string InvalidDescription = "Meals.InvalidDescription";
        public const string InvalidLocalDate = "Meals.InvalidLocalDate";
        public const string ItemNotFound = "Meals.ItemNotFound";
    }

    public static class MealItems
    {
        public const string InvalidMeal = "MealItems.InvalidMeal";
        public const string InvalidGrocery = "MealItems.InvalidGrocery";
        public const string InvalidQuantity = "MealItems.InvalidQuantity";
    }

    public static class UserExerciseLogs
    {
        public const string InvalidUser = "UserExerciseLogs.InvalidUser";
        public const string InvalidExercise = "UserExerciseLogs.InvalidExercise";
        public const string InvalidDuration = "UserExerciseLogs.InvalidDuration";
        public const string InvalidLocalDate = "UserExerciseLogs.InvalidLocalDate";
        public const string InvalidNotes = "UserExerciseLogs.InvalidNotes";
        public const string NotFound = "UserExerciseLogs.NotFound";
    }

}