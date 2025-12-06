using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryByCode
{
    public record GetGroceryByCodeQuery(string Code) : IRequest<ErrorOr<GroceryResult>>;
}
