using ErrorOr;
using MediatR;
using NutriTrack.Application.WeightHistoryEntries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.WeightHistoryEntries.Queries.ListWeightEntriesInRange;

public sealed record ListWeightHistoryEntriesInRangeQuery(
    UserId UserId,
    DateOnly From,
    DateOnly To) : IRequest<ErrorOr<IReadOnlyList<WeightHistoryEntryResult>>>;
