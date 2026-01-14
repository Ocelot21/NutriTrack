using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListGrocerySuggestions;

public sealed record ListGrocerySuggestionsQuery(int Page = 1, int PageSize = 20)
    : IRequest<ErrorOr<PagedResult<GroceryResult>>>;
