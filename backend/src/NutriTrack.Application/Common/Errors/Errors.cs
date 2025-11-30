using ErrorOr;
using NutriTrack.Domain.Common.Errors;

namespace NutriTrack.Application.Common.Errors
{
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
    }
}
