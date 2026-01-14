namespace NutriTrack.Contracts.AdminDashboard;

public sealed record GetAdminDashboardSummaryRequest(
    int Days = 7,
    int Top = 10);
