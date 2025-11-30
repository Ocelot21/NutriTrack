namespace NutriTrack.Contracts.Me;

public sealed record UpdateHealthProfileRequest(
    string? Gender,
    DateOnly? Birthdate,
    decimal? HeightCm,
    decimal? WeightKg,
    string? ActivityLevel
);
