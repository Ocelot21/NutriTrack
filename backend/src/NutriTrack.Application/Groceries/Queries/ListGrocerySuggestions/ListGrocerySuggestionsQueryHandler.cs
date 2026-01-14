using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Groceries.Queries.ListGrocerySuggestions;

public sealed class ListGrocerySuggestionsQueryHandler
    : IRequestHandler<ListGrocerySuggestionsQuery, ErrorOr<PagedResult<GroceryResult>>>
{
    private readonly IGroceryRepository _repo;
    private readonly IBlobStorageService _blobStorageService;

    public ListGrocerySuggestionsQueryHandler(IGroceryRepository repo, IBlobStorageService blobStorageService)
    {
        _repo = repo;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<GroceryResult>>> Handle(
        ListGrocerySuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedByApprovalAsync(
            isApproved: false,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken);

        var items = paged.Items
            .Select(g => g.ToGroceryResult())
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, r.ImageUrl) })
            .ToList();

        return new PagedResult<GroceryResult>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize);
    }
}
