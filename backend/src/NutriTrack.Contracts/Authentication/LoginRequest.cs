namespace NutriTrack.Contracts.Authentication;

public sealed record LoginRequest(
    string EmailOrUsername,
    string Password);