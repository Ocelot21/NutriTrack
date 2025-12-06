using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryByCode
{
    public class GetGroceryByCodeQueryHandler : IRequestHandler<GetGroceryByCodeQuery, ErrorOr<GroceryResult>>
    {
        private readonly IGroceryRepository _groceryRepository;

        public GetGroceryByCodeQueryHandler(IGroceryRepository groceryRepository)
        {
            _groceryRepository = groceryRepository;
        }

        public async Task<ErrorOr<GroceryResult>> Handle(GetGroceryByCodeQuery request, CancellationToken cancellationToken)
        {
            if (await _groceryRepository.GetByBarcodeAsync(request.Code) is not Grocery grocery)
            {
                return Error.NotFound(description: $"Grocery with barcode '{request.Code}' not found.");
            }

            return grocery.ToGroceryResult();
        }
    }
}