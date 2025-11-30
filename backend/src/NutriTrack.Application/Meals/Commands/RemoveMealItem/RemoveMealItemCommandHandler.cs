using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Meals.Commands.RemoveMealItem;

public sealed class RemoveMealItemCommandHandler : IRequestHandler<RemoveMealItemCommand, ErrorOr<Unit>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveMealItemCommandHandler(IMealRepository mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(RemoveMealItemCommand request, CancellationToken cancellationToken)
    {
        var meal = await _mealRepository.GetByIdAsync(request.MealId, cancellationToken);
        if (meal is null)
        {
            return Errors.Meals.NotFound;
        }

        meal.RemoveItem(request.MealItemId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
