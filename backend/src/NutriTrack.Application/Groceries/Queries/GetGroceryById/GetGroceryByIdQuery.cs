using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryById;

public record GetGroceryByIdQuery(GroceryId Id) : IRequest<ErrorOr<GroceryResult>>;
