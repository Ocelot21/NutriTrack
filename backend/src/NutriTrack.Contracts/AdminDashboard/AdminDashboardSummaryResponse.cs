namespace NutriTrack.Contracts.AdminDashboard;

public sealed record AdminDashboardSummaryResponse(
    AdminDashboardKpisResponse Kpis,
    IReadOnlyList<AdminDashboardDailyCountResponse> DailyCounts,
    IReadOnlyList<AdminDashboardTopItemResponse> TopGroceries,
    IReadOnlyList<AdminDashboardTopItemResponse> TopExercises);

public sealed record AdminDashboardKpisResponse(
    int ActiveUsers,
    int MealsLogged,
    int ExerciseLogsLogged);

public sealed record AdminDashboardDailyCountResponse(
    DateOnly Date,
    int MealsLogged,
    int ExerciseLogsLogged);

public sealed record AdminDashboardTopItemResponse(
    string Name,
    int Count);
