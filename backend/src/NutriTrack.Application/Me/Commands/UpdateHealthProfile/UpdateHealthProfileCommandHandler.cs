using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Users;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.WeightHistory;
using NutriTrack.Domain.ActivityLevelHistory;

namespace NutriTrack.Application.Me.Commands.UpdateHealthProfile;

public sealed class UpdateHealthProfileCommandHandler : IRequestHandler<UpdateHealthProfileCommand, ErrorOr<UserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly IActivityLevelHistoryRepository _activityLevelHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateHealthProfileCommandHandler(
        IUserRepository userRepository,
        IWeightHistoryRepository weightHistoryRepository,
        IActivityLevelHistoryRepository activityLevelHistoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _weightHistoryRepository = weightHistoryRepository;
        _activityLevelHistoryRepository = activityLevelHistoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<UserResult>> Handle(UpdateHealthProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var todayUtc = DateOnly.FromDateTime(utcNow);

        if (request.WeightKg.HasValue && user.WeightKg != request.WeightKg)
        {
            var entry = WeightHistoryEntry.Create(request.UserId, todayUtc, request.WeightKg.Value, utcNow);
            await _weightHistoryRepository.AddAsync(entry, cancellationToken);
        }

        if (request.ActivityLevel.HasValue && user.ActivityLevel != request.ActivityLevel.Value)
        {
            var entry = ActivityLevelHistoryEntry.Create(request.UserId, request.ActivityLevel.Value, todayUtc, utcNow);
            await _activityLevelHistoryRepository.AddAsync(entry, cancellationToken);
        }

        user.UpdateHealthProfile(
            request.Gender is null ? Optional<Gender>.None() : request.Gender.Value,
            request.Birthdate is null ? Optional<DateOnly?>.None() : request.Birthdate,
            request.HeightCm is null ? Optional<decimal?>.None() : request.HeightCm,
            request.WeightKg is null ? Optional<decimal?>.None() : request.WeightKg,
            request.ActivityLevel is null ? Optional<ActivityLevel>.None() : request.ActivityLevel.Value,
            request.NutritionGoal is null ? Optional<NutritionGoal>.None() : request.NutritionGoal.Value);

        user.MarkHealthProfileCompleted();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToUserResult();
    }
}