using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Exercises.Queries.ListExerciseSuggestions;

public sealed class ListExerciseSuggestionsQueryHandler
    : IRequestHandler<ListExerciseSuggestionsQuery, ErrorOr<PagedResult<ExerciseResult>>>
{
    private readonly IExerciseRepository _repo;
    private readonly IBlobStorageService _blobStorageService;

    public ListExerciseSuggestionsQueryHandler(IExerciseRepository repo, IBlobStorageService blobStorageService)
    {
        _repo = repo;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<ExerciseResult>>> Handle(
        ListExerciseSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedByApprovalAsync(
            isApproved: false,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken);

        var items = paged.Items
            .Select(e => e.ToExerciseResult())
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Exercises, r.ImageUrl) })
            .ToList();

        return new PagedResult<ExerciseResult>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize);
    }
}
