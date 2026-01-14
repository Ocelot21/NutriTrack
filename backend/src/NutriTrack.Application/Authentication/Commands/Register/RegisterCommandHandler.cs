using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Authentication;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Countries;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider  _dateTimeProvider;
    private readonly ITimeZoneService _timeZoneService;
    private readonly ICountryRepository _countryRepository;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        ITimeZoneService timeZoneService,
        ICountryRepository countryRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _timeZoneService = timeZoneService;
        _countryRepository = countryRepository;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (!_timeZoneService.TryNormalize(request.TimeZoneId, out var timeZone))
        {
            return Errors.Users.InvalidTimeZone;
        }

        var usernameVo = Username.Create(request.Username);
        var emailVo = Email.Create(request.Email);
        var countryCodeVo = CountryCode.CreateOptional(request.CountryIso2);

        if (await _userRepository.IsUsernameTakenAsync(usernameVo, cancellationToken))
        {
            return Errors.Authentication.UsernameAlreadyExists;
        }

        if (await _userRepository.GetByEmailAsync(emailVo, cancellationToken) is not null)
        {
            return Errors.Authentication.EmailAlreadyInUse;
        }
        
        if (countryCodeVo is not null 
            && !await _countryRepository.ExistsAsync(countryCodeVo, cancellationToken))
        {
            return Error.NotFound(
                code: "Country.NotFound",
                description: $"Country with ISO2 code '{countryCodeVo.Value}' was not found.");
        }

        var defaultRoleName = "User";

        if (await _roleRepository.GetByNameAsync(defaultRoleName, cancellationToken) is not Role role)
        {
            role = Role.Create(defaultRoleName, "Default role for regular users", true);
            await _roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            usernameVo.Value,
            emailVo.Value,
            hashedPassword,
            timeZone,
            role.Id,
            request.CountryIso2,
            isEmailVerified: false);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await _jwtTokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticationResult(token, false, null);
    }
}