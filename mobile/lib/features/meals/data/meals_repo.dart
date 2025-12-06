import '../../../core/api_client.dart';
import '../../../core/api_exception.dart';
import '../../../core/token_store.dart';

import 'create_meal_item_request.dart';
import 'create_meal_request.dart';
import 'meal_dropdown_models.dart';

class MealsException implements Exception {
  final String message;
  MealsException(this.message);

  @override
  String toString() => 'MealsException: $message';
}

class MealsRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  MealsRepo(this._api, this._tokenStore);

  String _dateOnlyString(DateTime dt) {
    final y = dt.year.toString().padLeft(4, '0');
    final m = dt.month.toString().padLeft(2, '0');
    final d = dt.day.toString().padLeft(2, '0');
    return '$y-$m-$d';
  }

  Future<void> createMeal(CreateMealRequest request) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<Map<String, dynamic>>(
        '/meals',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw MealsException(e.message);
    } catch (e) {
      throw MealsException('Unexpected error: $e');
    }
  }

  Future<void> updateMeal(String mealId, CreateMealRequest request) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.put<Map<String, dynamic>>(
        '/meals/$mealId',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw MealsException(e.message);
    } catch (e) {
      throw MealsException('Unexpected error: $e');
    }
  }

  Future<void> deleteMeal(String mealId) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>('/meals/$mealId');
    } on ApiException catch (e) {
      throw MealsException(e.message);
    } catch (e) {
      throw MealsException('Unexpected error: $e');
    }
  }

  Future<List<MealDropdownItem>> listMealsAroundDate({
    DateTime? centerDate,
    int daysBefore = 7,
    int daysAfter = 1,
  }) async {
    final center = centerDate ?? DateTime.now();
    final from = center.subtract(Duration(days: daysBefore));
    final to = center.add(Duration(days: daysAfter));

    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final query = <String, dynamic>{
        'From': _dateOnlyString(from),
        'To': _dateOnlyString(to),
      };

      final response = await _api.get<Map<String, dynamic>>(
        '/meals',
        queryParameters: query,
      );

      final data = response.data ?? {};
      final list = data['meals'] as List<dynamic>? ?? [];

      return list
          .map((e) => MealDropdownItem.fromJson(
        e as Map<String, dynamic>,
      ))
          .toList();
    } on ApiException catch (e) {
      throw Exception('Failed to load meals: ${e.message}');
    } catch (e) {
      throw Exception('Failed to load meals: $e');
    }
  }

  Future<void> createMealItem({
    required String mealId,
    required CreateMealItemRequest request,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>(
        '/meals/$mealId/items',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw Exception('Failed to create meal item: ${e.message}');
    } catch (e) {
      throw Exception('Failed to create meal item: $e');
    }
  }
}