using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Commands.CreateMeal;

public sealed class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, ErrorOr<MealResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeZoneService _timeZoneService;

    public CreateMealCommandHandler(
        IUserRepository userRepository,
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        ITimeZoneService timeZoneService)
    {
        _userRepository = userRepository;
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _timeZoneService = timeZoneService;
    }

    public async Task<ErrorOr<MealResult>> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) is not User user)
        {
            return Errors.Users.NotFound;
        }

        if (!_timeZoneService.TryNormalize(user.TimeZoneId, out string normalizedTimeZoneId))
        {
            return Errors.Users.InvalidTimeZone;
        }

        var occuredAtUtc = _timeZoneService.ToUtc(request.OccurredAtLocal, normalizedTimeZoneId);

        var localDate = DateOnly.FromDateTime(request.OccurredAtLocal.LocalDateTime);

        var meal = Meal.Create(
            request.UserId,
            request.Name,
            occuredAtUtc,
            request.OccurredAtLocal,
            localDate,
            request.Description);

        await _mealRepository.AddAsync(meal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return meal.ToMealResult();
    }
}