using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Application.Common.Mappings;

namespace NutriTrack.Application.Groceries.Queries.ListGroceriesByApproval;

public sealed class ListGroceriesByApprovalQueryHandler : IRequestHandler<ListGroceriesByApprovalQuery, ErrorOr<PagedResult<GroceryResult>>>
{
    private readonly IGroceryRepository _repo;

    public ListGroceriesByApprovalQueryHandler(IGroceryRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<PagedResult<GroceryResult>>> Handle(ListGroceriesByApprovalQuery request, CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedByApprovalAsync(request.IsApproved, request.Page, request.PageSize, cancellationToken);
        return new PagedResult<GroceryResult>(
            paged.Items.Select(g => g.ToGroceryResult()).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize);
    }
}
