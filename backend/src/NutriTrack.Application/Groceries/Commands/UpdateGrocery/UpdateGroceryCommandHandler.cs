using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.UpdateGrocery;

public sealed class UpdateGroceryCommandHandler : IRequestHandler<UpdateGroceryCommand, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public UpdateGroceryCommandHandler(
        IGroceryRepository groceryRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(UpdateGroceryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        if (request.Image is not null)
        {
            var oldBlobName = entity.ImageUrl;
            var newBlobName = await _blobStorageService.UploadAsync(
                BlobContainer.Groceries,
                request.Image,
                request.ImageFileName ?? "image",
                request.ImageContentType ?? "application/octet-stream",
                cancellationToken);

            entity.SetImage(newBlobName);

            await _blobStorageService.DeleteAsync(BlobContainer.Groceries, oldBlobName, cancellationToken);
        }

        entity.Update(
            request.Name is null ? Optional<string>.None() : request.Name,
            request.Category is null ? Optional<GroceryCategory>.None() : request.Category.Value,
            request.ProteinPer100.HasValue || request.CarbsPer100.HasValue || request.FatPer100.HasValue
                ? new MacroNutrients(
                    request.ProteinPer100 ?? entity.MacrosPer100.ProteinGramsPer100,
                    request.CarbsPer100 ?? entity.MacrosPer100.CarbsGramsPer100,
                    request.FatPer100 ?? entity.MacrosPer100.FatGramsPer100)
                : Optional<MacroNutrients>.None(),
            request.CaloriesPer100 is null ? Optional<int>.None() : request.CaloriesPer100.Value,
            request.UnitOfMeasure is null ? Optional<UnitOfMeasure>.None() : request.UnitOfMeasure.Value,
            request.GramsPerPiece is null ? Optional<decimal?>.None() : request.GramsPerPiece.Value,
            request.Barcode is null ? Optional<string>.None() : request.Barcode
           );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToGroceryResult();
    }
}
