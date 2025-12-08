import '../../../core/api_client.dart';
import '../../../core/api_exception.dart';
import '../../../core/token_store.dart';
import '../../../core/models/paged_response.dart';

import 'notification_models.dart';

class NotificationRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  NotificationRepo(this._api, this._tokenStore);

  Future<PagedResponse<AppNotification>> getNotifications({
    required int page,
    required int pageSize,
    bool? onlyUnread,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final query = <String, dynamic>{
        'Page': page,
        'PageSize': pageSize,
      };

      if (onlyUnread == true) {
        query['OnlyUnread'] = 'true';
      }

      final response = await _api.get<Map<String, dynamic>>(
        '/notifications',
        queryParameters: query,
      );

      final data = response.data ?? {};
      return PagedResponse<AppNotification>.fromJson(
        data,
            (json) => AppNotification.fromJson(json),
      );
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<void> markAsRead(String notificationId) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/notifications/$notificationId/read',
      );
    } on ApiException {
        // no need to handle
    }
  }

  Future<void> markAllAsRead() async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/notifications/mark-all-read',
      );
    } on ApiException {
      // no need to handle
    }
  }
}
