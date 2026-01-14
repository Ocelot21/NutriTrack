using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.Me.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.ChangeEmail(request.Email);
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var existing = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existing is not null && existing.Id != user.Id)
            {
                return Errors.Authentication.UsernameAlreadyExists;
            }
        }

        user.UpdateProfile(
            request.FirstName ?? Optional<string>.None(),
            request.LastName ?? Optional<string>.None(),
            request.Username ?? Optional<string>.None(),
            Optional<string>.None(),
            request.TimeZoneId ?? Optional<string>.None(),
            request.CountryIso2 ?? Optional<string>.None());

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}