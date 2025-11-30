using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public sealed class ListGroceriesQueryHandler : IRequestHandler<ListGroceriesQuery, ErrorOr<IReadOnlyList<GroceryResult>>>
{
    private readonly IGroceryRepository _groceryRepository;

    public ListGroceriesQueryHandler(IGroceryRepository groceryRepository)
    {
        _groceryRepository = groceryRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<GroceryResult>>> Handle(ListGroceriesQuery request, CancellationToken cancellationToken)
    {
        var list = await _groceryRepository.ListAsync(cancellationToken);
        return list.Select(g => g.ToGroceryResult()).ToList();
    }
}
