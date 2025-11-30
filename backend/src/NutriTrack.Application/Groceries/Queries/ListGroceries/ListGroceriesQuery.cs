using ErrorOr;
using MediatR;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public record ListGroceriesQuery(int? Page = null, int? PageSize = null) : IRequest<ErrorOr<IReadOnlyList<GroceryResult>>>;
