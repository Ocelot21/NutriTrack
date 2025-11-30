using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.UpdateGrocery;

public record UpdateGroceryCommand(
    GroceryId Id,
    string? Name,
    GroceryCategory? Category,
    decimal? ProteinPer100g,
    decimal? CarbsPer100g,
    decimal? FatPer100g,
    int? CaloriesPer100,
    UnitOfMeasure? UnitOfMeasure,
    string? Barcode
) : IRequest<ErrorOr<GroceryResult>>;
