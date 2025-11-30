using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Commands.UpdateHealthProfile;

public sealed class UpdateHealthProfileCommandHandler : IRequestHandler<UpdateHealthProfileCommand, ErrorOr<UserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHealthProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserResult>> Handle(UpdateHealthProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        user.UpdateHealthProfile(
            request.Gender is null ? Optional<Gender>.None() : request.Gender.Value,
            request.Birthdate,
            request.HeightCm,
            request.WeightKg,
            request.ActivityLevel is null ? Optional<ActivityLevel>.None() : request.ActivityLevel.Value);

        user.MarkHealthProfileCompleted();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToUserResult();
    }
}
