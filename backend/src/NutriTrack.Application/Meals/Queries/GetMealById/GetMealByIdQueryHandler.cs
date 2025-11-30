using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Meals.Common;

namespace NutriTrack.Application.Meals.Queries.GetMealById;

public sealed class GetMealByIdQueryHandler : IRequestHandler<GetMealByIdQuery, ErrorOr<MealResult>>
{
    private readonly IMealRepository _mealRepository;

    public GetMealByIdQueryHandler(IMealRepository mealRepository)
    {
        _mealRepository = mealRepository;
    }

    public async Task<ErrorOr<MealResult>> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
    {
        var meal = await _mealRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meal is null)
        {
            return Errors.Meals.NotFound;
        }

        return meal.ToMealResult();
    }
}
