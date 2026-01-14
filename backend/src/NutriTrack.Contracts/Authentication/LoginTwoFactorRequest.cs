namespace NutriTrack.Contracts.Authentication;

public sealed record LoginTwoFactorRequest(
    Guid ChallengeId,
    string Code
);
