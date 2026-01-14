namespace NutriTrack.Contracts.Me;

public sealed record UpdateUserProfileRequest(
    string? Email,
    string? Username,
    string? FirstName,
    string? LastName,
    string? TimeZoneId,
    string? CountryIso2
);
