using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Meals.Commands.DeleteMeal;

public sealed class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, ErrorOr<Unit>>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMealCommandHandler(IMealRepository mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMealCommand request, CancellationToken cancellationToken)
    {
        var entity = await _mealRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Meals.NotFound;
        }

        _mealRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
