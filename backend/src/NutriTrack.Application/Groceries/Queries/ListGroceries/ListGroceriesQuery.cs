using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public record ListGroceriesQuery(
    UserId? UserId,
    GroceryListFilters Filters,
    int? Page = 1,
    int? PageSize = 10) : IRequest<ErrorOr<PagedResult<GroceryResult>>>;
