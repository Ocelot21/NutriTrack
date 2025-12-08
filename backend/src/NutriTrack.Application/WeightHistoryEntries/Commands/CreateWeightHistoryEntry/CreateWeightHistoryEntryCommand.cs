using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.WeightHistoryEntries.Commands.CreateWeightHistoryEntry;

public sealed record CreateWeightHistoryEntryCommand(
    UserId UserId,
    DateOnly Date,
    decimal WeightKg
) : IRequest<ErrorOr<Unit>>;
