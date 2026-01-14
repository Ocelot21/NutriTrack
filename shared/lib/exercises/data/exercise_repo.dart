import 'package:dio/dio.dart';
import 'package:cross_file/cross_file.dart';
import 'package:nutritrack_shared/exercises/data/update_user_exercise_log_request.dart';

import '../../../../core/api_client.dart';
import '../../../../core/api_exception.dart';
import '../../../../core/token_store.dart';
import '../../core/models/paged_response.dart';
import 'create_user_exercise_log_request.dart';
import 'exercise_models.dart';
import 'create_exercise_request.dart';

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

  Future<void> createExercise({
    required String name,
    required ExerciseCategory category,
    required double defaultCaloriesPerMinute,
    String? description,
    XFile? imageFile,
  }) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final map = <String, dynamic>{
      'Name': name,
      'Category': category.backendValue,
      'DefaultCaloriesPerMinute': defaultCaloriesPerMinute.toString(),
      'Description': description,
    };

    if (imageFile != null) {
      map['Image'] = await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.name,
      );
    }

    final formData = FormData.fromMap(map);

    await _api.postMultipart<void>('/exercises', formData);
  }

  Future<Exercise> getById(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/exercises/$id',
      );

      final data = response.data ?? {};
      return Exercise.fromJson(data);
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }

  Future<void> updateExercise({
    required String id,
    required String name,
    required ExerciseCategory category,
    required double defaultCaloriesPerMinute,
    String? description,
    XFile? imageFile,
  }) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final map = <String, dynamic>{
      'Name': name,
      'Category': category.backendValue,
      'DefaultCaloriesPerMinute': defaultCaloriesPerMinute.toString(),
      'Description': description,
    };

    if (imageFile != null) {
      map['Image'] = await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.name,
      );
    }

    final formData = FormData.fromMap(map);

    await _api.putMultipart<void>('/exercises/$id', formData);
  }

  Future<void> deleteExercise(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>('/exercises/$id');
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

  Future<PagedResponse<Exercise>> listSuggestionExercises({
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/exercises/suggestions',
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
        },
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

  Future<Exercise> suggestExercise({
    required String name,
    required ExerciseCategory category,
    required double defaultCaloriesPerMinute,
    String? description,
    XFile? imageFile,
  }) async {
    return suggestExerciseRequest(
      CreateExerciseRequest(
        name: name,
        category: category,
        defaultCaloriesPerMinute: defaultCaloriesPerMinute,
        description: description,
        imageFile: imageFile,
      ),
    );
  }

  Future<Exercise> suggestExerciseRequest(CreateExerciseRequest request) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final map = <String, dynamic>{
        'Name': request.name,
        'Category': request.category.backendValue,
        'DefaultCaloriesPerMinute': request.defaultCaloriesPerMinute.toString(),
        'Description': request.description,
      };

      final imageFile = request.imageFile;
      if (imageFile != null) {
        map['Image'] = await MultipartFile.fromFile(
          imageFile.path,
          filename: imageFile.name,
        );
      }

      final formData = FormData.fromMap(map);

      final response = await _api.postMultipart<Map<String, dynamic>>(
        '/exercises/suggestions',
        formData,
      );

      // The API returns an ExerciseResponse; this shared lib maps it into [Exercise].
      return Exercise.fromJson(response);
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }

  Future<void> approveSuggestion(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>('/exercises/suggestions/$id/approve');
    } on ApiException catch (e) {
      throw ExerciseSearchException(e.message);
    } catch (e) {
      throw ExerciseSearchException('Unexpected error: $e');
    }
  }
}