using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.CreateGrocery;

public record CreateGroceryCommand(
    string Name,
    GroceryCategory Category,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    int CaloriesPer100,
    UnitOfMeasure UnitOfMeasure,
    string? Barcode,
    string? ImageUrl
) : IRequest<ErrorOr<GroceryResult>>;
