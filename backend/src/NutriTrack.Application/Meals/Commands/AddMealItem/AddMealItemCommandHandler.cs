using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.AddMealItem;

public sealed class AddMealItemCommandHandler : IRequestHandler<AddMealItemCommand, ErrorOr<Unit>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddMealItemCommandHandler(IMealRepository mealRepository, IGroceryRepository groceryRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(AddMealItemCommand request, CancellationToken cancellationToken)
    {
        if (await _mealRepository.GetByIdAsync(request.MealId, cancellationToken) is not Meal meal)
        { 
            return Errors.Meals.NotFound;
        }

        if (await _groceryRepository.GetByIdAsync(request.GroceryId, cancellationToken) is not Grocery grocery)
        {
            return Errors.Groceries.NotFound;
        }

        meal.AddItem(grocery, request.Quantity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}