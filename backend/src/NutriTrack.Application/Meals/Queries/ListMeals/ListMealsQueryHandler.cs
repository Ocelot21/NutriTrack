using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Meals.Common;

namespace NutriTrack.Application.Meals.Queries.ListMeals;

public sealed class ListMealsQueryHandler : IRequestHandler<ListMealsQuery, ErrorOr<IReadOnlyList<MealResult>>>
{
    private readonly IMealRepository _mealRepository;

    public ListMealsQueryHandler(IMealRepository mealRepository)
    {
        _mealRepository = mealRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<MealResult>>> Handle(ListMealsQuery request, CancellationToken cancellationToken)
    {
        if (request.From.HasValue && request.To.HasValue)
        {
            var meals = await _mealRepository
                .GetByUserAndDateRangeAsync(request.UserId, request.From.Value, request.To.Value, cancellationToken);
            return meals.Select(m => m.ToMealResult()).ToList();
        }

        var all = await _mealRepository.ListAsync(cancellationToken);
        return all.Where(m => m.UserId == request.UserId).Select(m => m.ToMealResult()).ToList();
    }
}