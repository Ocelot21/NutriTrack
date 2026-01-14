import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import 'daily_overview_models.dart';

class HealthProfileNotCompletedException implements Exception {}

class DailyOverviewException implements Exception {
  final String message;
  DailyOverviewException(this.message);

  @override
  String toString() => 'DailyOverviewException: $message';
}

class DailyOverviewRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  DailyOverviewRepo(this._api, this._tokenStore);

  Future<DailyOverview> getDailyOverview(DateTime date) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final dateStr =
          '${date.year.toString().padLeft(4, '0')}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}';

      final response = await _api.get<Map<String, dynamic>>(
        '/me/daily-overview/$dateStr',
      );

      final data = response.data ?? {};
      return DailyOverview.fromJson(data);
    } on ApiException catch (e) {
      final errorKeys = e.problem?.errorKeys ?? const [];

      if (errorKeys.contains('Users.HealthProfileNotCompleted')) {
        throw HealthProfileNotCompletedException();
      }

      throw DailyOverviewException(e.message);

    } catch (e) {
      throw DailyOverviewException('Unexpected error: $e');
    }
  }
}