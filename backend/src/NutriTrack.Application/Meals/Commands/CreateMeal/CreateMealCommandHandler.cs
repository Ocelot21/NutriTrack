using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.CreateMeal;

public sealed class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, ErrorOr<MealResult>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMealCommandHandler(IMealRepository mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<MealResult>> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        var entity = Meal.Create(
            request.UserId,
            request.Name,
            request.OccurredAtUtc,
            request.OccurredAtLocal,
            request.LocalDate,
            request.Description);

        await _mealRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToMealResult();
    }
}
