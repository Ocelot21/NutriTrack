import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';

import 'user_achievement_models.dart';

class UserAchievementRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  UserAchievementRepo(this._api, this._tokenStore);

  Future<PagedResponse<UserAchievementModel>> getUserAchievements({
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/user-achievements',
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
        },
      );

      final data = response.data ?? {};
      return PagedResponse<UserAchievementModel>.fromJson(
        data,
            (json) => UserAchievementModel.fromJson(json),
      );
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }
}
