using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListGroceriesByApproval;

public sealed record ListGroceriesByApprovalQuery(bool IsApproved, int Page, int PageSize) : IRequest<ErrorOr<PagedResult<GroceryResult>>>;
