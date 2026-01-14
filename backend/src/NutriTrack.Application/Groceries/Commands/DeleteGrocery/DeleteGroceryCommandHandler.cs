using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.DeleteGrocery;

public sealed class DeleteGroceryCommandHandler : IRequestHandler<DeleteGroceryCommand, ErrorOr<Unit>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteGroceryCommandHandler(
        IGroceryRepository groceryRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteGroceryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        var blobName = entity.ImageUrl;
        _groceryRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _blobStorageService.DeleteAsync(BlobContainer.Groceries, blobName, cancellationToken);
        return Unit.Value;
    }
}
