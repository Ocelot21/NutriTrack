using ErrorOr;
using MediatR;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.DeleteGrocery;

public record DeleteGroceryCommand(GroceryId Id) : IRequest<ErrorOr<Unit>>;
