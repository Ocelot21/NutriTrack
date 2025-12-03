using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private User() : base() { }

    private User(
        UserId id,
        string firstName,
        string lastName,
        Username username,
        Email email,
        string passwordHash,
        RoleId roleId,
        bool isEmailVerified,
        string timeZoneId,
        CountryCode? country)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsEmailVerified = isEmailVerified;
        TimeZoneId = timeZoneId;
        Country = country;
    }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Username Username { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public bool IsEmailVerified { get; private set; }

    public RoleId RoleId { get; private set; } = default!;
    public Role? Role { get; private set; }

    public string? AvatarUrl { get; private set; }
    public string TimeZoneId { get; private set; } = null!;
    public DateTime? LastLoginAtUtc { get; private set; }
    public CountryCode? Country { get; private set; }

    public bool IsHealthProfileCompleted { get; private set; }

    public Gender Gender { get; private set; } = Gender.Unknown;
    public ActivityLevel ActivityLevel { get; private set; } = ActivityLevel.Sedentary;

    public DateOnly? Birthdate { get; private set; }

    public decimal? HeightCm { get; private set; }
    public decimal? WeightKg { get; private set; }

    public NutritionGoal NutritionGoal { get; private set; } = NutritionGoal.MaintainWeight;


    // --------- Factory ---------

    public static User Create(
        string firstName,
        string lastName,
        string username,
        string email,
        string passwordHash,
        string timeZoneId,
        RoleId roleId,
        string? countryIso2 = null,
        bool isEmailVerified = false,
        Gender gender = Gender.Unknown,
        DateOnly? birthdate = null,
        decimal? heightCm = null,
        decimal? weightKg = null,
        ActivityLevel activityLevel = ActivityLevel.Sedentary)
    {
        firstName = NormalizeName(firstName, isFirstName: true);
        lastName = NormalizeName(lastName, isFirstName: false);
        var usernameVo = Username.Create(username);
        var emailVo = Email.Create(email);
        passwordHash = NormalizePasswordHash(passwordHash);
        timeZoneId = NormalizeTimeZoneId(timeZoneId);
        var countryCode = CountryCode.CreateOptional(countryIso2);

        // TODO: Add health profile normalization if creating pre-completed profile.

        return new User(
            new UserId(Guid.NewGuid()),
            firstName,
            lastName,
            usernameVo,
            emailVo,
            passwordHash,
            roleId,
            isEmailVerified,
            timeZoneId,
            countryCode);
    }

    // --------- Domain methods ---------

    public void UpdateProfile(
        Optional<string> firstName,
        Optional<string> lastName,
        Optional<string> username,
        Optional<string> avatarUrl,
        Optional<string> timeZoneId,
        Optional<string> countryIso2)
    {
        if (firstName.IsSet)
        {
            FirstName = NormalizeName(firstName.Value, isFirstName: true);
        }

        if (lastName.IsSet)
        {
            LastName = NormalizeName(lastName.Value, isFirstName: false);
        }

        if (username.IsSet)
        {
            Username = Username.Create(username.Value);
        }

        if (avatarUrl.IsSet)
        {
            var value = avatarUrl.Value?.Trim();
            AvatarUrl = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        if (timeZoneId.IsSet)
        {
            TimeZoneId = NormalizeTimeZoneId(timeZoneId.Value);
        }

        if (countryIso2.IsSet)
        {
            Country = CountryCode.CreateOptional(countryIso2.Value);
        }
    }

    public void UpdateHealthProfile(
        Optional<Gender> gender,
        Optional<DateOnly?> birthdate,
        Optional<decimal?> heightCm,
        Optional<decimal?> weightKg,
        Optional<ActivityLevel> activityLevel,
        Optional<NutritionGoal> nutritionGoal)
    {
        if (gender.IsSet)
        {
            ValidateGender(gender.Value);
            Gender = gender.Value;
        }

        if (birthdate.IsSet)
        {
            Birthdate = NormalizeBirthdate(birthdate.Value);
        }

        if (heightCm.IsSet)
        {
            HeightCm = NormalizeHeight(heightCm.Value);
        }

        if (weightKg.IsSet)
        {
            WeightKg = NormalizeWeight(weightKg.Value);
        }

        if (activityLevel.IsSet)
        {
            ValidateActivityLevel(activityLevel.Value);
            ActivityLevel = activityLevel.Value;
        }

        if (nutritionGoal.IsSet)
        {
            ValidateNutritionGoal(nutritionGoal.Value);
            NutritionGoal = nutritionGoal.Value;
        }
    }

    public void ChangeEmail(string newEmail)
    {
        var emailVo = Email.Create(newEmail);

        if (emailVo.Value == Email.Value)
        {
            return;
        }

        Email = emailVo;
        IsEmailVerified = false;
    }

    public void MarkEmailVerified()
    {
        IsEmailVerified = true;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = NormalizePasswordHash(newPasswordHash);
    }

    public void ChangeRole(RoleId newRoleId)
    {
        if (newRoleId == RoleId)
        {
            return;
        }

        RoleId = newRoleId;
    }

    public void MarkLoggedIn(DateTime utcNow)
    {
        LastLoginAtUtc = utcNow;
    }

    public void ChangeTimeZone(string timeZoneId)
    {
        TimeZoneId = NormalizeTimeZoneId(timeZoneId);
    }

    public void MarkHealthProfileCompleted()
    {
        IsHealthProfileCompleted = true;
    }


    // --------- Private helpers ---------

    private static string NormalizeName(string value, bool isFirstName)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            var code = isFirstName
                ? DomainErrorCodes.Users.InvalidFirstName
                : DomainErrorCodes.Users.InvalidLastName;

            var label = isFirstName ? "First name" : "Last name";

            throw new DomainException(
                code,
                $"{label} cannot be empty.");
        }

        if (value.Length > DomainConstraints.Users.MaxNameLength)
        {
            var code = isFirstName
                ? DomainErrorCodes.Users.InvalidFirstName
                : DomainErrorCodes.Users.InvalidLastName;

            var label = isFirstName ? "First name" : "Last name";

            throw new DomainException(
                code,
                $"{label} cannot be longer than {DomainConstraints.Users.MaxNameLength} characters.");
        }

        return value;
    }

    private static string NormalizePasswordHash(string passwordHash)
    {
        passwordHash = passwordHash.Trim();

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidPasswordHash,
                "Password hash cannot be empty.");
        }

        return passwordHash;
    }

    private static string NormalizeTimeZoneId(string timeZoneId)
    {
        timeZoneId = timeZoneId.Trim();

        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidTimeZone,
                "Time zone identifier cannot be empty.");
        }

        // TODO: validate against a list of supported time zone identifiers.

        return timeZoneId;
    }

    private static DateOnly? NormalizeBirthdate(DateOnly? birthdate)
    {
        if (!birthdate.HasValue)
        {
            return null;
        }

        var value = birthdate.Value;

        if (value.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidBirthdate,
                "Birthdate cannot be in the future.");
        }

        if (value.Year < 1900)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidBirthdate,
                "Birthdate is unrealistically old.");
        }

        return value;
    }

    private static decimal? NormalizeHeight(decimal? heightCm)
    {
        if (!heightCm.HasValue)
        {
            return null;
        }

        if (heightCm.Value <= 0 || heightCm.Value > 300)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidHeight,
                "Height must be between 1 and 300 cm.");
        }

        return heightCm.Value;
    }

    private static decimal? NormalizeWeight(decimal? weightKg)
    {
        if (!weightKg.HasValue)
        {
            return null;
        }

        if (weightKg.Value <= 0 || weightKg.Value > 500)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidWeight,
                "Weight must be between 1 and 500 kg.");
        }

        return weightKg.Value;
    }

    private static void ValidateGender(Gender gender)
    {
        if (!Enum.IsDefined(typeof(Gender), gender))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidGender,
                "Gender value is invalid.");
        }
    }

    private static void ValidateActivityLevel(ActivityLevel activityLevel)
    {
        if (!Enum.IsDefined(typeof(ActivityLevel), activityLevel))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidActivityLevel,
                "Activity level value is invalid.");
        }
    }

    private static void ValidateNutritionGoal(NutritionGoal nutritionGoal)
    {
        if (!Enum.IsDefined(typeof(NutritionGoal), nutritionGoal))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidActivityLevel,
                "Activity level value is invalid.");
        }
    }
}