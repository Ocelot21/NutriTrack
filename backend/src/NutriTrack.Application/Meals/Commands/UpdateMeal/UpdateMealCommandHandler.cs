using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Commands.UpdateMeal;

public sealed class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, ErrorOr<Unit>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeZoneService _timeZoneService;

    public UpdateMealCommandHandler(
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ITimeZoneService timeZoneService)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _timeZoneService = timeZoneService;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        if (await _mealRepository.GetByIdAsync(request.Id, cancellationToken) is not Meal meal)
        {
            return Errors.Meals.NotFound;
        }

        if (await _userRepository.GetByIdAsync(meal.UserId, cancellationToken) is not User user)
        {
            return Errors.Users.NotFound;
        }

        if (user.Id != request.UserId)
        {
            return Errors.Authorization.Unauthorized;
        }

        DateTime? occurredAtUtc = null;
        DateOnly? localDate = null;

        if (request.OccurredAtLocal.HasValue)
        {
            if (!_timeZoneService.TryNormalize(user.TimeZoneId, out string normalizedTimeZoneId))
            {
                return Errors.Users.InvalidTimeZone;
            }

            occurredAtUtc = _timeZoneService.ToUtc(request.OccurredAtLocal.Value, normalizedTimeZoneId);

            localDate = DateOnly.FromDateTime(request.OccurredAtLocal.Value.DateTime);
        }
        

        meal.UpdateDetails(
            request.Name is null ? Optional<string>.None() : request.Name,
            request.Description is null ? Optional<string?>.None() : request.Description,
            occurredAtUtc is null ? Optional<DateTime>.None() : occurredAtUtc.Value,
            request.OccurredAtLocal is null ? Optional<DateTimeOffset>.None() : request.OccurredAtLocal.Value,
            localDate is null ? Optional<DateOnly>.None() : localDate.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}