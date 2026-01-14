using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Groceries.Queries.ListRecommendedGroceries;

public sealed record ListRecommendedGroceriesQuery(
    UserId UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<ErrorOr<PagedResult<GroceryResult>>>;
