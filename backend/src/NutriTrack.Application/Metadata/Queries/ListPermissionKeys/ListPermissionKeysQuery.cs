using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Metadata.Queries.ListPermissionKeys;

public sealed record ListPermissionKeysQuery() : IRequest<ErrorOr<IReadOnlyList<string>>>;
