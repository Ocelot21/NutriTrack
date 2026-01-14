using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.ApproveGrocery;

public sealed record ApproveGroceryCommand(GroceryId Id) : IRequest<ErrorOr<GroceryResult>>;
