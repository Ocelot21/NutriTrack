using NutriTrack.Contracts.AdminDashboard;

namespace NutriTrack.Application.AdminDashboard;

public interface IAdminDashboardService
{
    Task<AdminDashboardSummaryResponse> GetSummaryAsync(
        GetAdminDashboardSummaryRequest request,
        CancellationToken cancellationToken = default);
}
