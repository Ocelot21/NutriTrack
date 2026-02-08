using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;

namespace NutriTrack.Application.Groceries.Queries.ListEnhancedRecommendedGroceries;

public sealed class ListEnhancedRecommendedGroceriesQueryHandler
    : IRequestHandler<ListEnhancedRecommendedGroceriesQuery, ErrorOr<PagedResult<GroceryRecommendationResult>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IEnhancedGroceryRecommender _enhancedRecommender;
    private readonly IBlobStorageService _blobStorageService;

    public ListEnhancedRecommendedGroceriesQueryHandler(
        IUserRepository userRepository,
        IGroceryRepository groceryRepository,
        IEnhancedGroceryRecommender enhancedRecommender,
        IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _groceryRepository = groceryRepository;
        _enhancedRecommender = enhancedRecommender;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<GroceryRecommendationResult>>> Handle(
        ListEnhancedRecommendedGroceriesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        // If user has no health profile, return fallback with basic recommendations
        if (!user.IsHealthProfileCompleted)
        {
            var fallback = await _groceryRepository.GetPagedAsync(
                filters: new GroceryListFilters(null, null, null, null, null, null, null, null, null, null, null),
                userId: request.UserId,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            var fallbackItems = fallback.Items
                .Select(g => new GroceryRecommendationResult(
                    g.Id,
                    g.Name,
                    g.Category,
                    g.Barcode,
                    g.MacrosPer100,
                    g.CaloriesPer100,
                    g.UnitOfMeasure,
                    g.GramsPerPiece,
                    _blobStorageService.GenerateReadUri(BlobContainer.Groceries, g.ImageUrl),
                    g.IsApproved,
                    g.IsDeleted,
                    0.0,
                    "Complete your health profile for personalized recommendations"))
                .ToList();

            return new PagedResult<GroceryRecommendationResult>(
                fallbackItems, 
                fallback.TotalCount, 
                fallback.Page, 
                fallback.PageSize);
        }

        var recommended = await _enhancedRecommender.GetRecommendedAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mappedItems = recommended.Items
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Groceries, r.ImageUrl) })
            .ToList();

        return new PagedResult<GroceryRecommendationResult>(
            mappedItems, 
            recommended.TotalCount, 
            recommended.Page, 
            recommended.PageSize);
    }
}
