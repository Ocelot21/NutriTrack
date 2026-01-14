import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import 'user_goal_models.dart';

class UserGoalException implements Exception {
  final String message;
  UserGoalException(this.message);

  @override
  String toString() => 'UserGoalException: $message';
}

class UserGoalRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  UserGoalRepo(this._api, this._tokenStore);

  Future<void> _attachToken() async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);
  }

  Future<UserGoalDto?> getCurrentGoal() async {
    try {
      await _attachToken();

      final response = await _api.get<Map<String, dynamic>>(
        '/user-goals/current',
      );

      final data = response.data;
      if (data == null) return null;

      return UserGoalDto.fromJson(data);
    } on ApiException catch (e) {
      if (e.statusCode == 404) {
        return null;
      }

      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }


  Future<void> createGoal({
    required NutritionGoalTypeUi type,
    required DateTime targetDate,
    required double targetWeightKg,
  }) async {
    try {
      await _attachToken();

      final body = <String, dynamic>{
        'type': type.backendValue,
        'targetDate': targetDate.toIso8601String().split('T').first,
        'targetWeightKg': targetWeightKg,
      };

      await _api.post<void>(
        '/user-goals',
        data: body,
      );
    } on ApiException catch (e) {
      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }

  Future<void> cancelGoal(String id) async {
    try {
      await _attachToken();

      await _api.post<void>(
        '/user-goals/$id/cancel',
      );
    } on ApiException catch (e) {
      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }


  Future<void> createWeightEntry({
    required DateTime date,
    required double weightKg,
  }) async {
    try {
      await _attachToken();

      final body = <String, dynamic>{
        'date': date.toIso8601String().split('T').first,
        'weightKg': weightKg,
      };

      await _api.post<void>(
        '/weight-history-entries',
        data: body,
      );
    } on ApiException catch (e) {
      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }

  Future<List<WeightHistoryEntryDto>> listWeightHistory({
    required DateTime from,
    required DateTime to,
  }) async {
    try {
      await _attachToken();

      final query = <String, dynamic>{
        'from': from.toIso8601String().split('T').first,
        'to': to.toIso8601String().split('T').first,
      };

      final response = await _api.get<Map<String, dynamic>>(
        '/weight-history-entries',
        queryParameters: query,
      );

      final data = response.data ?? <String, dynamic>{};
      final rawEntries = data['entries'];

      if (rawEntries is! List) {
        return <WeightHistoryEntryDto>[];
      }

      return rawEntries
          .map((e) => WeightHistoryEntryDto.fromJson(
        e as Map<String, dynamic>,
      ))
          .toList();
    } on ApiException catch (e) {
      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }

  Future<PagedResponse<UserGoalDto>> listGoals({
    required int page,
    required int pageSize,
  }) async {
    try {
      await _attachToken();

      final query = <String, dynamic>{
        'Page': page,
        'PageSize': pageSize,
      };

      final response = await _api.get<Map<String, dynamic>>(
        '/user-goals',
        queryParameters: query,
      );

      final data = response.data ?? <String, dynamic>{};

      return PagedResponse<UserGoalDto>.fromJson(
        data,
            (j) => UserGoalDto.fromJson(j),
      );
    } on ApiException catch (e) {
      throw UserGoalException(e.message);
    } catch (e) {
      throw UserGoalException('Unexpected error: $e');
    }
  }

}
