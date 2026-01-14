using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Metadata.Queries.GetSystemTimeZones;

public record GetSystemTimeZonesQuery() : IRequest<ErrorOr<IReadOnlyList<string>>>;