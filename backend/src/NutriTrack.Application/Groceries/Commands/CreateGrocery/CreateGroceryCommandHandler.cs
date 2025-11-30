using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.CreateGrocery;

public sealed class CreateGroceryCommandHandler : IRequestHandler<CreateGroceryCommand, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroceryCommandHandler(IGroceryRepository groceryRepository, IUnitOfWork unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(CreateGroceryCommand request, CancellationToken cancellationToken)
    {
        var macros = new MacroNutrients(request.ProteinPer100g, request.CarbsPer100g, request.FatPer100g);
        var barcodeOpt = string.IsNullOrWhiteSpace(request.Barcode) ? Optional<string>.None() : request.Barcode!;

        var entity = Grocery.Create(
            request.Name,
            request.Category,
            macros,
            request.CaloriesPer100,
            request.UnitOfMeasure,
            barcodeOpt,
            request.ImageUrl);

        await _groceryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ToGroceryResult();
    }
}
