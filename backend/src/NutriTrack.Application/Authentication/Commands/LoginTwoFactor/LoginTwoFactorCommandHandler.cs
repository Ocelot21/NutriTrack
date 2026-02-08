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

    private readonly IPendingLoginChallengeRepository _pendingLoginChallengeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITotpSecretProtector _totpSecretProtector;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly ITimeZoneService _timeZoneService;

    public LoginTwoFactorCommandHandler(
        IPendingLoginChallengeRepository pendingLoginChallengeRepository,
        IUserRepository userRepository,
        ITotpSecretProtector totpSecretProtector,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IUserGoalRepository userGoalRepository,
        ITimeZoneService timeZoneService)
    {
        _pendingLoginChallengeRepository = pendingLoginChallengeRepository;
        _userRepository = userRepository;
        _totpSecretProtector = totpSecretProtector;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _userGoalRepository = userGoalRepository;
        _timeZoneService = timeZoneService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(
        LoginTwoFactorCommand request,
        CancellationToken cancellationToken)
    {
        var challenge = await _pendingLoginChallengeRepository.GetAsync(request.ChallengeId, cancellationToken);
        if (challenge is null)
            return Errors.Authentication.InvalidCredentials;

        if (challenge.Consumed || challenge.ExpiresAtUtc <= _dateTimeProvider.UtcNow)
            return Errors.Authentication.TwoFactorExpired;

        if (challenge.Attempts >= MaxAttempts)
            return Errors.Authentication.TooManyAttempts;

        var user = await _userRepository.GetByIdAsync(challenge.UserId, cancellationToken);

        if (user is null || !user.IsTwoFactorEnabled || user.TotpSecretProtected is null)
            return Errors.Authentication.InvalidCredentials;

        var secret = _totpSecretProtector.Unprotect(user.TotpSecretProtected);
        var totp = new Totp(Base32Encoding.ToBytes(secret));

        if (!totp.VerifyTotp(request.Code, out _, new VerificationWindow(1, 1)))
        {
            await _pendingLoginChallengeRepository.IncrementAttemptsAsync(challenge.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Errors.Authentication.InvalidTwoFactorCode;
        }

        await _pendingLoginChallengeRepository.ConsumeAsync(challenge.Id, cancellationToken);

        if (user.IsHealthProfileCompleted && user.WeightKg.HasValue)
        {
            var currentGoal = await _userGoalRepository.GetCurrentForUser(user.Id);
            if (currentGoal is not null)
            {
                var currentDate = _timeZoneService.LocalDate(_dateTimeProvider.UtcNow, user.TimeZoneId);
                currentGoal.EvaluateProgress(user.WeightKg.Value, currentDate, _dateTimeProvider.UtcNow);
            }
        }

        user.MarkLoggedIn(_dateTimeProvider.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await _jwtTokenGenerator.GenerateTokenAsync(user, cancellationToken);

        return new AuthenticationResult(
            AccessToken: token,
            RequiresTwoFactor: false,
            TwoFactorChallengeId: null);
    }
}
