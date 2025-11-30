using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public record ListGroceriesQuery(int? Page = null, int? PageSize = null) : IRequest<ErrorOr<PagedResult<GroceryResult>>>;
