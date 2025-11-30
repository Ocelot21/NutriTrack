using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Authentication;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Email emailVo = Email.Create(request.Email);

        var user = await _userRepository.GetByEmailAsync(emailVo, cancellationToken);

        if (user is null)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Errors.Authentication.InvalidCredentials;
        }


        user.MarkLoggedIn(_dateTimeProvider.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await _jwtTokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticationResult(token);
    }
}