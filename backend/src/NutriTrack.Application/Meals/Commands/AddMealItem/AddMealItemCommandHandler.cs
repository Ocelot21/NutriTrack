using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Meals.Commands.AddMealItem;

public sealed class AddMealItemCommandHandler : IRequestHandler<AddMealItemCommand, ErrorOr<Unit>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddMealItemCommandHandler(IMealRepository mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(AddMealItemCommand request, CancellationToken cancellationToken)
    {
        var meal = await _mealRepository.GetByIdAsync(request.MealId, cancellationToken);
        if (meal is null)
        {
            return Errors.Meals.NotFound;
        }

        meal.AddItem(request.Grocery, request.Quantity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
