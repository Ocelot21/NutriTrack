using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryById;

public sealed class GetGroceryByIdQueryHandler : IRequestHandler<GetGroceryByIdQuery, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;

    public GetGroceryByIdQueryHandler(IGroceryRepository groceryRepository)
    {
        _groceryRepository = groceryRepository;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(GetGroceryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        return entity.ToGroceryResult();
    }
}
