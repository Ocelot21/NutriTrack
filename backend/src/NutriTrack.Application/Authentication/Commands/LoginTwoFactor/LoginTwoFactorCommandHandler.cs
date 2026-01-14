namespace NutriTrack.Application.Authentication.Commands.LoginTwoFactor;

using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Authentication;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Application.Common.Interfaces.Services;
using OtpNet;

public sealed class LoginTwoFactorCommandHandler
    : IRequestHandler<LoginTwoFactorCommand, ErrorOr<AuthenticationResult>>
{
    private const int MaxAttempts = 5;

    private readonly IPendingLoginChallengeRepository _challenges;
    private readonly IUserRepository _users;
    private readonly ITotpSecretProtector _protector;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _time;
    private readonly IUserGoalRepository _userGoals;
    private readonly ITimeZoneService _timeZoneService;

    public LoginTwoFactorCommandHandler(
        IPendingLoginChallengeRepository challenges,
        IUserRepository users,
        ITotpSecretProtector protector,
        IJwtTokenGenerator jwt,
        IUnitOfWork uow,
        IDateTimeProvider time,
        IUserGoalRepository userGoals,
        ITimeZoneService timeZoneService)
    {
        _challenges = challenges;
        _users = users;
        _protector = protector;
        _jwt = jwt;
        _uow = uow;
        _time = time;
        _userGoals = userGoals;
        _timeZoneService = timeZoneService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(
        LoginTwoFactorCommand request,
        CancellationToken ct)
    {
        var challenge = await _challenges.GetAsync(request.ChallengeId, ct);
        if (challenge is null)
            return Errors.Authentication.InvalidCredentials;

        if (challenge.Consumed || challenge.ExpiresAtUtc <= _time.UtcNow)
            return Errors.Authentication.TwoFactorExpired;

        if (challenge.Attempts >= MaxAttempts)
            return Errors.Authentication.TooManyAttempts;

        var user = await _users.GetByIdAsync(challenge.UserId, ct);

        if (user is null || !user.IsTwoFactorEnabled || user.TotpSecretProtected is null)
            return Errors.Authentication.InvalidCredentials;

        var secret = _protector.Unprotect(user.TotpSecretProtected);
        var totp = new Totp(Base32Encoding.ToBytes(secret));

        if (!totp.VerifyTotp(request.Code, out _, new VerificationWindow(1, 1)))
        {
            await _challenges.IncrementAttemptsAsync(challenge.Id, ct);
            await _uow.SaveChangesAsync(ct);
            return Errors.Authentication.InvalidTwoFactorCode;
        }

        await _challenges.ConsumeAsync(challenge.Id, ct);

        if (user.IsHealthProfileCompleted && user.WeightKg.HasValue)
        {
            var currentGoal = await _userGoals.GetCurrentForUser(user.Id);
            if (currentGoal is not null)
            {
                var currentDate = _timeZoneService.LocalDate(_time.UtcNow, user.TimeZoneId);
                currentGoal.EvaluateProgress(user.WeightKg.Value, currentDate, _time.UtcNow);
            }
        }

        user.MarkLoggedIn(_time.UtcNow);
        await _uow.SaveChangesAsync(ct);

        var token = await _jwt.GenerateTokenAsync(user, ct);

        return new AuthenticationResult(
            AccessToken: token,
            RequiresTwoFactor: false,
            TwoFactorChallengeId: null);
    }
}
