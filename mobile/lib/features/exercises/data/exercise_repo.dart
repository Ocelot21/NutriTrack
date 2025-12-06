import '../../../core/api_client.dart';
import '../../../core/api_exception.dart';
import '../../../core/token_store.dart';

import 'create_user_exercise_log_request.dart';
import 'exercise_models.dart';
import 'update_user_exercise_log_request.dart';

class ExerciseSearchFilters {
  final String? searchTerm;
  final ExerciseCategory? category;
  final double? minCaloriesPerMinute;
  final double? maxCaloriesPerMinute;

  const ExerciseSearchFilters({
    this.searchTerm,
    this.category,
    this.minCaloriesPerMinute,
    this.maxCaloriesPerMinute,
  });

  ExerciseSearchFilters copyWith({
    String? searchTerm,
    ExerciseCategory? category,
    double? minCaloriesPerMinute,
    double? maxCaloriesPerMinute,
  }) {
    return ExerciseSearchFilters(
      searchTerm: searchTerm ?? this.searchTerm,
      category: category ?? this.category,
      minCaloriesPerMinute:
      minCaloriesPerMinute ?? this.minCaloriesPerMinute,
      maxCaloriesPerMinute:
      maxCaloriesPerMinute ?? this.maxCaloriesPerMinute,
    );
  }
}

class ExerciseSearchException implements Exception {
  final String message;
  ExerciseSearchException(this.message);

  @override
  String toString() => 'ExerciseSearchException: $message';
}

class ExerciseRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  ExerciseRepo(this._api, this._tokenStore);

  Future<PagedResponse<Exercise>> searchExercises({
    required ExerciseSearchFilters filters,
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

      if (filters.searchTerm != null &&
          filters.searchTerm!.trim().isNotEmpty) {
        query['searchTerm'] = filters.searchTerm!.trim();
      }

      if (filters.category != null) {
        query['category'] = filters.category!.backendValue;
      }

      if (filters.minCaloriesPerMinute != null) {
        query['minCaloriesPerMinute'] =
            filters.minCaloriesPerMinute!.toString();
      }

      if (filters.maxCaloriesPerMinute != null) {
        query['maxCaloriesPerMinute'] =
            filters.maxCaloriesPerMinute!.toString();
      }

      final response = await _api.get<Map<String, dynamic>>(
        '/exercises',
        queryParameters: query,
      );

      final data = response.data ?? {};
      return PagedResponse<Exercise>.fromJson(
        data,
            (j) => Exercise.fromJson(j),
      );
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }

  Future<void> createUserExerciseLog(
      CreateUserExerciseLogRequest request,
      ) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/user-exercise-logs',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }

  Future<void> updateUserExerciseLog(
      String logId,
      UpdateUserExerciseLogRequest request,
      ) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.put<void>(
        '/user-exercise-logs/$logId',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }

  Future<void> deleteUserExerciseLog(String logId) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>('/user-exercise-logs/$logId');
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }
}
