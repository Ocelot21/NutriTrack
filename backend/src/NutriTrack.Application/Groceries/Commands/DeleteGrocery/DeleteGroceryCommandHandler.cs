using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.DeleteGrocery;

public sealed class DeleteGroceryCommandHandler : IRequestHandler<DeleteGroceryCommand, ErrorOr<Unit>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGroceryCommandHandler(IGroceryRepository groceryRepository, IUnitOfWork unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteGroceryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        _groceryRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
