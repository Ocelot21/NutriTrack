namespace NutriTrack.Contracts.Me;

public sealed record MeResponse(
    string Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string? Role,
    bool IsEmailVerified,
    string? AvatarUrl,
    string TimeZoneId,
    DateTime? LastLoginAtUtc,
    string? Country,
    bool IsHealthProfileCompleted,
    string Gender,
    string ActivityLevel,
    DateOnly? Birthdate,
    decimal? HeightCm,
    decimal? WeightKg
);
