using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Metadata.Queries.ListPermissionKeys;

public sealed class ListPermissionKeysQueryHandler : IRequestHandler<ListPermissionKeysQuery, ErrorOr<IReadOnlyList<string>>>
{
    public Task<ErrorOr<IReadOnlyList<string>>> Handle(ListPermissionKeysQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> keys = PermissionKeys.All
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return Task.FromResult<ErrorOr<IReadOnlyList<string>>>(keys.ToList());
    }
}
