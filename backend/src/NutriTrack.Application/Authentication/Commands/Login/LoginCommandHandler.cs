using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Authentication;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPendingTotpSetupRepository _pendingTotpSetupRepository;
    private readonly ITotpSecretProtector _totpSecretProtector;
    private readonly IPendingLoginChallengeRepository _pendingLoginChallengeRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IAchievementService _achievementService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        IPendingTotpSetupRepository pendingTotpSetupRepository,
        ITotpSecretProtector totpSecretProtector,
        IPendingLoginChallengeRepository pendingLoginChallengeRepository,
        IUserGoalRepository userGoalRepository,
        ITimeZoneService timeZoneService,
        IAchievementService achievementService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _pendingTotpSetupRepository = pendingTotpSetupRepository;
        _totpSecretProtector = totpSecretProtector;
        _pendingLoginChallengeRepository = pendingLoginChallengeRepository;
        _userGoalRepository = userGoalRepository;
        _timeZoneService = timeZoneService;
        _achievementService = achievementService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {

        if (DomainPatterns.EmailRegex.IsMatch(request.EmailOrUsername))
        {
            Email emailVo = Email.Create(request.EmailOrUsername);

            if (await _userRepository.GetByEmailAsync(
                emailVo, 
                cancellationToken) is not User user)
            {
                return Errors.Authentication.InvalidCredentials;
            }

            return await CompleteLogin(user, request.Password, cancellationToken);
        }

        if (!DomainPatterns.UsernameRegex.IsMatch(request.EmailOrUsername))
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var usernameVo = Username.Create(request.EmailOrUsername);
        
        if (await _userRepository.GetByUsernameAsync(
            usernameVo.Value, 
            cancellationToken) is not User userByName)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        return await CompleteLogin(userByName, request.Password, cancellationToken);
    }

    private async Task<ErrorOr<AuthenticationResult>> CompleteLogin(
        User user,
        string password,
        CancellationToken cancellationToken)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Errors.Authentication.InvalidCredentials;

        if (user.IsTwoFactorEnabled)
        {
            var challenge = new PendingLoginChallenge(
                Id: Guid.NewGuid(),
                UserId: user.Id,
                ExpiresAtUtc: _dateTimeProvider.UtcNow.AddMinutes(5),
                Attempts: 0,
                Consumed: false);

            await _pendingLoginChallengeRepository.AddAsync(challenge, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthenticationResult(
                AccessToken: null,
                RequiresTwoFactor: true,
                TwoFactorChallengeId: challenge.Id);
        }

        var utcNow = _dateTimeProvider.UtcNow;
        user.MarkLoggedIn(utcNow);

        if (user.IsHealthProfileCompleted && user.WeightKg.HasValue)
        {
            var currentGoal = await _userGoalRepository.GetCurrentForUser(user.Id);
            if (currentGoal is not null)
            {
                var currentDate = _timeZoneService.LocalDate(utcNow, user.TimeZoneId);
                currentGoal.EvaluateProgress(user.WeightKg.Value, currentDate, utcNow);
            }
        }

        await _achievementService.CheckGoalCompletedAsync(user.Id, cancellationToken);
        await _achievementService.CheckMealItemLoggedAsync(user.Id, cancellationToken);
        await _achievementService.CheckExerciseLoggedAsync(user.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await _jwtTokenGenerator.GenerateTokenAsync(user, cancellationToken);

        return new AuthenticationResult(
            AccessToken: token,
            RequiresTwoFactor: false,
            TwoFactorChallengeId: null);
    }


}