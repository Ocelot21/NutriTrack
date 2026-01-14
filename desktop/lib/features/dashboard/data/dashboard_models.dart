/// Mirrors backend contracts in `NutriTrack.Contracts.AdminDashboard`.
///
/// Endpoint:
/// GET /api/admin/dashboard/summary?days=7&top=10
class AdminDashboardSummaryResponse {
  final AdminDashboardKpisResponse kpis;
  final List<AdminDashboardDailyCountResponse> dailyCounts;
  final List<AdminDashboardTopItemResponse> topGroceries;
  final List<AdminDashboardTopItemResponse> topExercises;

  const AdminDashboardSummaryResponse({
    required this.kpis,
    required this.dailyCounts,
    required this.topGroceries,
    required this.topExercises,
  });

  factory AdminDashboardSummaryResponse.fromJson(Map<String, dynamic> json) {
    return AdminDashboardSummaryResponse(
      kpis: AdminDashboardKpisResponse.fromJson(
        (json['kpis'] as Map?)?.cast<String, dynamic>() ?? const <String, dynamic>{},
      ),
      dailyCounts: (json['dailyCounts'] as List<dynamic>? ?? const [])
          .whereType<Map>()
          .map((e) => AdminDashboardDailyCountResponse.fromJson(
                e.cast<String, dynamic>(),
              ))
          .toList(growable: false),
      topGroceries: (json['topGroceries'] as List<dynamic>? ?? const [])
          .whereType<Map>()
          .map((e) => AdminDashboardTopItemResponse.fromJson(
                e.cast<String, dynamic>(),
              ))
          .toList(growable: false),
      topExercises: (json['topExercises'] as List<dynamic>? ?? const [])
          .whereType<Map>()
          .map((e) => AdminDashboardTopItemResponse.fromJson(
                e.cast<String, dynamic>(),
              ))
          .toList(growable: false),
    );
  }
}

class AdminDashboardKpisResponse {
  final int activeUsers;
  final int mealsLogged;
  final int exerciseLogsLogged;

  const AdminDashboardKpisResponse({
    required this.activeUsers,
    required this.mealsLogged,
    required this.exerciseLogsLogged,
  });

  factory AdminDashboardKpisResponse.fromJson(Map<String, dynamic> json) {
    int asInt(dynamic v) => (v as num?)?.toInt() ?? 0;

    return AdminDashboardKpisResponse(
      activeUsers: asInt(json['activeUsers']),
      mealsLogged: asInt(json['mealsLogged']),
      exerciseLogsLogged: asInt(json['exerciseLogsLogged']),
    );
  }
}

/// Backend type is DateOnly; the API typically serializes it as `YYYY-MM-DD`.
/// We keep it as a `DateTime` (at midnight UTC/local depending on parsing).
class AdminDashboardDailyCountResponse {
  final DateTime date;
  final int mealsLogged;
  final int exerciseLogsLogged;

  const AdminDashboardDailyCountResponse({
    required this.date,
    required this.mealsLogged,
    required this.exerciseLogsLogged,
  });

  factory AdminDashboardDailyCountResponse.fromJson(Map<String, dynamic> json) {
    int asInt(dynamic v) => (v as num?)?.toInt() ?? 0;

    final dateStr = json['date']?.toString() ?? '';
    final parsedDate = DateTime.tryParse(dateStr) ?? DateTime.fromMillisecondsSinceEpoch(0);

    return AdminDashboardDailyCountResponse(
      date: parsedDate,
      mealsLogged: asInt(json['mealsLogged']),
      exerciseLogsLogged: asInt(json['exerciseLogsLogged']),
    );
  }
}

class AdminDashboardTopItemResponse {
  final String name;
  final int count;

  const AdminDashboardTopItemResponse({
    required this.name,
    required this.count,
  });

  factory AdminDashboardTopItemResponse.fromJson(Map<String, dynamic> json) {
    return AdminDashboardTopItemResponse(
      name: json['name']?.toString() ?? '',
      count: (json['count'] as num?)?.toInt() ?? 0,
    );
  }
}

class GetAdminDashboardSummaryRequest {
  final int days;
  final int top;

  const GetAdminDashboardSummaryRequest({
    this.days = 7,
    this.top = 10,
  });

  Map<String, dynamic> toQuery() => {
        'days': days,
        'top': top,
      };
}

