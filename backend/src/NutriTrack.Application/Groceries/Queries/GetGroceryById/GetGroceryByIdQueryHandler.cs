using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryById;

public sealed class GetGroceryByIdQueryHandler : IRequestHandler<GetGroceryByIdQuery, ErrorOr<GroceryResult>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetGroceryByIdQueryHandler(IGroceryRepository groceryRepository, IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<GroceryResult>> Handle(GetGroceryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _groceryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Groceries.NotFound;
        }

        var result = entity.ToGroceryResult();
        return result with
        {
            ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, result.ImageUrl)
        };
    }
}
