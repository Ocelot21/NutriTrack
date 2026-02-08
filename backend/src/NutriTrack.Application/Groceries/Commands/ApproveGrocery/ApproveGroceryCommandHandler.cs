using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Commands.ApproveGrocery;

public sealed class ApproveGroceryCommandHandler
    : IRequestHandler<ApproveGroceryCommand, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public ApproveGroceryCommandHandler(
        IGroceryRepository groceryRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(ApproveGroceryCommand request, CancellationToken cancellationToken)
    {
        var grocery = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (grocery is null)
        {
            return Errors.Groceries.NotFound;
        }

        grocery.Approve();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = grocery.ToGroceryResult();
        return result with
        {
            ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, result.ImageUrl)
        };
    }
}
