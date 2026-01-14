using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.WeightHistoryEntries.Common;

namespace NutriTrack.Application.WeightHistoryEntries.Queries.ListWeightEntriesInRange;

public sealed class ListWeightHistoryEntriesInRangeQueryHandler 
    : IRequestHandler<ListWeightHistoryEntriesInRangeQuery, ErrorOr<IReadOnlyList<WeightHistoryEntryResult>>>
{
    private readonly IWeightHistoryRepository _weightHistoryRepository;

    public ListWeightHistoryEntriesInRangeQueryHandler(
        IWeightHistoryRepository weightHistoryRepository)
    {
        _weightHistoryRepository = weightHistoryRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<WeightHistoryEntryResult>>> Handle(
        ListWeightHistoryEntriesInRangeQuery request,
        CancellationToken cancellationToken)
    {
        var entries = await _weightHistoryRepository.ListAsync(cancellationToken);

        var filtered = entries
            .Where(e => e.UserId == request.UserId && e.Date >= request.From && e.Date <= request.To)
            .Select(e => new WeightHistoryEntryResult(e.Id, e.UserId, e.Date, e.WeightKg))
            .ToList();
        return filtered;
    }
}