import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';

import 'social_models.dart';

class SocialException implements Exception {
  final String message;
  SocialException(this.message);

  @override
  String toString() => 'SocialException: $message';
}

class SocialRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  SocialRepo(this._api, this._tokenStore);

  Future<PagedResponse<SocialPostModel>> getFeed({
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final query = <String, dynamic>{
        'page': page,
        'pageSize': pageSize,
      };

      final response = await _api.get<Map<String, dynamic>>(
        '/me/social/feed',
        queryParameters: query,
      );

      final data = response.data ?? {};
      return PagedResponse<SocialPostModel>.fromJson(
        data,
            (j) => SocialPostModel.fromJson(j),
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  Future<void> createTextPost({
    required String text,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/me/social/posts/text',
        data: {
          'text': text,
        },
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  Future<void> shareAchievement({
    required String userAchievementId,
    String? caption,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/me/social/posts/achievement/$userAchievementId',
        data: {
          'caption': caption,
        },
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  Future<void> shareGoalProgressSnapshot({
    required String userGoalId,
    String? caption,
    int? visibility,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/me/social/posts/goal-progress/$userGoalId',
        data: {
          'caption': caption,
          'visibility': visibility,
        },
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  /// [localDate] should be formatted as yyyy-MM-dd to match backend DateOnly route binding.
  Future<void> shareDailyOverviewSnapshot({
    required String localDate,
    String? caption,
    int? visibility,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/me/social/posts/daily-overview/$localDate',
        data: {
          'caption': caption,
          'visibility': visibility,
        },
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  Future<void> deletePost({
    required String postId,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>(
        '/me/social/posts/$postId',
      );
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }

  Future<SocialProfileModel> getProfile({
    required String userId,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/me/social/profile/$userId',
      );

      return SocialProfileModel.fromJson(response.data ?? {});
    } on ApiException catch (e) {
      throw SocialException(e.message);
    } catch (e) {
      throw SocialException('Unexpected error: $e');
    }
  }
}
