namespace NutriTrack.Application.Authentication.Common;

public sealed record AuthenticationResult(
    string? AccessToken,
    bool RequiresTwoFactor,
    Guid? TwoFactorChallengeId
    );
