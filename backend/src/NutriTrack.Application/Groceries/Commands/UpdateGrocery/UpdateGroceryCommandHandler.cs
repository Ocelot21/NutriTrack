using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.UpdateGrocery;

public sealed class UpdateGroceryCommandHandler : IRequestHandler<UpdateGroceryCommand, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGroceryCommandHandler(IGroceryRepository groceryRepository, IUnitOfWork unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(UpdateGroceryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        entity.Update(
            request.Name is null ? Optional<string>.None() : request.Name,
            request.Category is null ? Optional<GroceryCategory>.None() : request.Category.Value,
            request.ProteinPer100g.HasValue || request.CarbsPer100g.HasValue || request.FatPer100g.HasValue
                ? new MacroNutrients(
                    request.ProteinPer100g ?? entity.MacrosPer100.ProteinGramsPer100g,
                    request.CarbsPer100g ?? entity.MacrosPer100.CarbsGramsPer100g,
                    request.FatPer100g ?? entity.MacrosPer100.FatGramsPer100g)
                : Optional<MacroNutrients>.None(),
            request.CaloriesPer100 is null ? Optional<int>.None() : request.CaloriesPer100.Value,
            request.Barcode is null ? Optional<string>.None() : request.Barcode,
            request.UnitOfMeasure is null ? Optional<UnitOfMeasure>.None() : request.UnitOfMeasure.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToGroceryResult();
    }
}
