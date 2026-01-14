namespace NutriTrack.Contracts.Authentication;

public sealed record AuthenticationResponse(
    string? AccessToken,
    bool RequiresTwoFactor,
    Guid? TwoFactorChallengeId);