using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;
using System.IO;

namespace NutriTrack.Application.Groceries.Commands.CreateGrocery;

public record CreateGroceryCommand(
    string Name,
    GroceryCategory Category,
    decimal ProteinPer100,
    decimal CarbsPer100,
    decimal FatPer100,
    int CaloriesPer100,
    UnitOfMeasure UnitOfMeasure,
    decimal? GramsPerPiece,
    string? Barcode,
    Stream? Image,
    string? ImageFileName,
    string? ImageContentType
) : IRequest<ErrorOr<GroceryResult>>;
