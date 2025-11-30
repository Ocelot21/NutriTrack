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
        IReadOnlyList<Grocery> list;


        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            var pagedResult = await _groceryRepository.ListAsync(
                request.Page.Value,
                request.PageSize.Value,
                cancellationToken
            );
            list = pagedResult.Items;

            return new PagedResult<GroceryResult>(
                list.Select(g => g.ToGroceryResult()).ToList(),
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount
            );
        }
        else
        {
            list = await _groceryRepository.ListAsync(cancellationToken);
        }

        return new PagedResult<GroceryResult>(
            list.Select(g => g.ToGroceryResult()).ToList(),
            request.Page ?? 0,
            request.PageSize ?? 0,
            list.Count
        );
    }
}