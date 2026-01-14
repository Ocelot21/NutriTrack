import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import 'dashboard_models.dart';

class DashboardException implements Exception {
  final String message;
  DashboardException(this.message);

  @override
  String toString() => 'DashboardException: $message';
}

class DashboardRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  DashboardRepo(this._api, this._tokenStore);

  /// GET /api/admin/dashboard/summary
  Future<AdminDashboardSummaryResponse> getSummary({
    int days = 7,
    int top = 10,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/admin/dashboard/summary',
        queryParameters: GetAdminDashboardSummaryRequest(days: days, top: top).toQuery(),
      );

      final data = response.data ?? const <String, dynamic>{};
      return AdminDashboardSummaryResponse.fromJson(data);
    } on ApiException catch (e) {
      throw DashboardException(e.message);
    } catch (e) {
      throw DashboardException('Unexpected error: $e');
    }
  }
}

