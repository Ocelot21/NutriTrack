using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.ActivityLevelHistoryEntries.Commands.CreateActivityLevelHistoryEntry;

public sealed class CreateActivityLevelHistoryEntryCommandHandler
    : IRequestHandler<CreateActivityLevelHistoryEntryCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IActivityLevelHistoryRepository _activityLevelHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateActivityLevelHistoryEntryCommandHandler(
        IUserRepository userRepository,
        IActivityLevelHistoryRepository activityLevelHistoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _activityLevelHistoryRepository = activityLevelHistoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateActivityLevelHistoryEntryCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var entry = ActivityLevelHistoryEntry.Create(request.UserId, request.ActivityLevel, request.EffectiveFrom, utcNow);

        user.UpdateHealthProfile(
            gender: Optional<Domain.Users.Gender>.None(),
            birthdate: Optional<DateOnly?>.None(),
            heightCm: Optional<decimal?>.None(),
            weightKg: Optional<decimal?>.None(),
            activityLevel: request.ActivityLevel,
            nutritionGoal: Optional<Domain.Users.NutritionGoal>.None());

        await _activityLevelHistoryRepository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
