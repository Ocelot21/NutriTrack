using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public sealed class ListExercisesQueryHandler : IRequestHandler<ListExercisesQuery, ErrorOr<PagedResult<ExerciseResult>>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IBlobStorageService _blobStorageService;

    public ListExercisesQueryHandler(IExerciseRepository exerciseRepository, IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<ExerciseResult>>> Handle(ListExercisesQuery request, CancellationToken cancellationToken)
    {
        var list = await _exerciseRepository.GetPagedAsync(
            request.Filters,
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var paged = list.ToPagedResult();

        var items = paged.Items
            .Select(r => r with { ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Exercises, r.ImageUrl) })
            .ToList();

        return new PagedResult<ExerciseResult>(items, paged.TotalCount, paged.Page, paged.PageSize);
    }
}