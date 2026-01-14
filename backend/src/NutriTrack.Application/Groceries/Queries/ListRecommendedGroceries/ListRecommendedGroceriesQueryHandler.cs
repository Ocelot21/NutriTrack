using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListRecommendedGroceries;

public sealed class ListRecommendedGroceriesQueryHandler
    : IRequestHandler<ListRecommendedGroceriesQuery, ErrorOr<PagedResult<GroceryResult>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IGroceryRecommender _recommender;
    private readonly IBlobStorageService _blobStorageService;

    public ListRecommendedGroceriesQueryHandler(
        IUserRepository userRepository,
        IGroceryRepository groceryRepository,
        IGroceryRecommender recommender,
        IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _groceryRepository = groceryRepository;
        _recommender = recommender;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<GroceryResult>>> Handle(
        ListRecommendedGroceriesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        // If user has no health profile, skip ML and return the usual list.
        if (!user.IsHealthProfileCompleted)
        {
            var fallback = await _groceryRepository.GetPagedAsync(
                filters: new GroceryListFilters(null, null, null, null, null, null, null, null, null, null, null),
                userId: request.UserId,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            var paged = fallback.ToGroceryPagedResult();
            var items = paged.Items
                .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, r.ImageUrl) })
                .ToList();
            return new PagedResult<GroceryResult>(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        var recommended = await _recommender.GetRecommendedAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mappedItems = recommended.Items
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, r.ImageUrl) })
            .ToList();
        return new PagedResult<GroceryResult>(mappedItems, recommended.TotalCount, recommended.Page, recommended.PageSize);
    }
}
