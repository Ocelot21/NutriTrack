using ErrorOr;
using NutriTrack.Domain.Common.Errors;

namespace NutriTrack.Application.Common.Errors;

public static class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.InvalidCredentials,
                description: "The provided credentials are invalid.");

        public static Error EmailAlreadyInUse =>
            Error.Conflict(
                code: DomainErrorCodes.Authentication.EmailAlreadyInUse,
                description: "Email is already in use.");

        public static Error UsernameAlreadyExists =>
            Error.Conflict(
                code: DomainErrorCodes.Authentication.UsernameAlreadyExists,
                description: "Username is already taken.");

        public static Error PasswordsDoNotMatch =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.PasswordsDoNotMatch,
                description: "The provided passwords do not match.");

        public static Error InvalidPassword =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.InvalidPassword,
                description: "The provided password is incorrect.");

        public static Error TwoFactorExpired =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.TwoFactorExpired,
                description: "The two-factor authentication code has expired.");

        public static Error TooManyAttempts =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.TooManyAttempts,
                description: "Too many login attempts. Please try again later.");

        public static Error InvalidTwoFactorCode =>
            Error.Validation(
                code: DomainErrorCodes.Authentication.InvalidTwoFactorCode,
                description: "The provided two-factor authentication code is invalid.");
    }

    public static class Authorization
    {
        public static Error Unauthorized =>
            Error.Unauthorized(
                code: DomainErrorCodes.Authorization.Unauthorized,
                description: "You are not authorized to perform this action.");

    }

    public static class Roles
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.Roles.NotFound,
                description: "Role was not found.");
    }

    public static class Users
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.Users.NotFound,
                description: "User was not found.");

        public static Error InvalidEmail =>
            Error.Validation(
                code: DomainErrorCodes.Users.InvalidEmail,
                description: "Email is invalid.");

        public static Error InvalidTimeZone =>
            Error.Validation(
                code: DomainErrorCodes.Users.InvalidTimeZone,
                description: "Time zone identifier is invalid.");

        public static Error InvalidUsername =>
            Error.Validation(
                code: DomainErrorCodes.Users.InvalidUsername,
                description: "Username is invalid.");

        public static Error HealthProfileNotCompleted =>
            Error.Validation(
                code: DomainErrorCodes.Users.HealthProfileNotCompleted,
                description: "User's health profile is not completed.");
    }

    public static class Groceries
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.Groceries.NotFound,
                description: "Grocery was not found.");
    }

    public static class Exercises
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.Exercises.NotFound,
                description: "Exercise was not found.");
    }

    public static class Meals
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.Meals.InvalidName,
                description: "Meal was not found.");
    }

    public static class UserGoals
    {
        public static Error NotFound =>
            Error.NotFound(
                code: DomainErrorCodes.UserGoals.NotFound,
                description: "User goal was not found.");

    }

    public static class SocialPosts
    {
        public static Error NotFound =>
            Error.NotFound(
                code: "SocialPosts.NotFound",
                description: "Social post was not found.");

        public static Error Forbidden =>
            Error.Unauthorized(
                code: "SocialPosts.Forbidden",
                description: "You are not authorized to perform this action.");
    }
}