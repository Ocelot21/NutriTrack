using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.AdminDashboard;
using NutriTrack.Contracts.AdminDashboard;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Api.Controllers;

[Route("api/admin/dashboard")]
public sealed class AdminDashboardController : ApiController
{
    private readonly IAdminDashboardService _adminDashboardService;

    public AdminDashboardController(IAdminDashboardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }

    [Authorize(Policy = PermissionKeys.Users.Manage)]
    [HttpGet("summary")]
    public async Task<ActionResult<AdminDashboardSummaryResponse>> GetSummary(
        [FromQuery] GetAdminDashboardSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _adminDashboardService.GetSummaryAsync(request, cancellationToken);
        return Ok(response);
    }
}
