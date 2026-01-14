namespace NutriTrack.Contracts.Totp;

public sealed record TotpSetupResponse(
    string OtpauthUri,
    string ManualKeyBase32);
