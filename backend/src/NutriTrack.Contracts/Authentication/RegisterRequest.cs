namespace NutriTrack.Contracts.Authentication;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string ConfirmPassword,
    string? CountryIso2,
    string TimeZoneId);