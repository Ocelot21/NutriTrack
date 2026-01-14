using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public sealed class ListGroceriesQueryHandler : IRequestHandler<ListGroceriesQuery, ErrorOr<PagedResult<GroceryResult>>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IBlobStorageService _blobStorageService;

    public ListGroceriesQueryHandler(IGroceryRepository groceryRepository, IBlobStorageService blobStorageService)
    {
        _groceryRepository = groceryRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<GroceryResult>>> Handle(ListGroceriesQuery request, CancellationToken cancellationToken)
    {
        var paged = (await _groceryRepository.GetPagedAsync(
            request.Filters,
            request.UserId,
            request.Page ?? 1,
            request.PageSize ?? 10,
            cancellationToken))
            .ToGroceryPagedResult();

        var items = paged.Items
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, r.ImageUrl) })
            .ToList();

        return new PagedResult<GroceryResult>(items, paged.TotalCount, paged.Page, paged.PageSize);
    }
}