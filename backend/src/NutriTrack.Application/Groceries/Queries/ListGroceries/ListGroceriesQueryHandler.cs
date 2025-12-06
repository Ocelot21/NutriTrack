using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public sealed class ListGroceriesQueryHandler : IRequestHandler<ListGroceriesQuery, ErrorOr<PagedResult<GroceryResult>>>
{
    private readonly IGroceryRepository _groceryRepository;

    public ListGroceriesQueryHandler(IGroceryRepository groceryRepository)
    {
        _groceryRepository = groceryRepository;
    }

    public async Task<ErrorOr<PagedResult<GroceryResult>>> Handle(ListGroceriesQuery request, CancellationToken cancellationToken)
    {
        return (await _groceryRepository.GetPagedAsync(
            request.Filters,
            request.UserId,
            request.Page ?? 1,
            request.PageSize ?? 10,
            cancellationToken))
            .ToGroceryPagedResult();
    }
}