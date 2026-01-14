using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.CreateGrocerySuggestion;

public sealed class CreateGrocerySuggestionCommandHandler
    : IRequestHandler<CreateGrocerySuggestionCommand, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public CreateGrocerySuggestionCommandHandler(
        IGroceryRepository groceryRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(
        CreateGrocerySuggestionCommand request,
        CancellationToken cancellationToken)
    {
        var macros = new MacroNutrients(request.ProteinPer100, request.CarbsPer100, request.FatPer100);

        string? imageBlobName = null;
        if (request.Image is not null)
        {
            imageBlobName = await _blobStorageService.UploadAsync(
                BlobContainer.Groceries,
                request.Image,
                request.ImageFileName ?? "image",
                request.ImageContentType ?? "application/octet-stream",
                cancellationToken);
        }

        var entity = Grocery.Create(
            request.Name,
            request.Category,
            macros,
            request.CaloriesPer100,
            request.UnitOfMeasure,
            request.GramsPerPiece,
            request.Barcode,
            imageBlobName,
            isApproved: false);

        await _groceryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ToGroceryResult();
    }
}
