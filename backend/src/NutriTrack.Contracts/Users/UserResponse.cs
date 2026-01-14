namespace NutriTrack.Contracts.Users;

public sealed record UserResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    Guid? RoleId,
    string? RoleName,
    bool IsEmailVerified,
    string? AvatarUrl,
    string TimeZoneId,
    DateTime? LastLoginAtUtc,
    string? CountryCode,
    bool IsHealthProfileCompleted,
    string Gender,
    string ActivityLevel,
    DateOnly? Birthdate,
    decimal? HeightCm,
    decimal? WeightKg,
    bool IsTwoFactorEnabled
);
