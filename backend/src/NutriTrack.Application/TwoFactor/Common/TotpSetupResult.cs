namespace NutriTrack.Application.TwoFactor.Common;

public record TotpSetupResult(
    string OtpauthUri,
    string ManualKeyBase32);
