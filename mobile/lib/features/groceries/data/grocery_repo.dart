import '../../../core/api_client.dart';
import '../../../core/api_exception.dart';
import '../../../core/token_store.dart';
import '../../../core/models/paged_response.dart';

import 'grocery_models.dart';

class GrocerySearchFilters {
  final String? searchTerm;
  final GroceryCategory? category;
  final UnitOfMeasureUi? unitOfMeasure;
  final double? minCaloriesPer100;
  final double? maxCaloriesPer100;
  final double? minProteinPer100g;
  final double? maxProteinPer100g;
  final double? minCarbsPer100g;
  final double? maxCarbsPer100g;
  final double? minFatPer100g;
  final double? maxFatPer100g;

  const GrocerySearchFilters({
    this.searchTerm,
    this.category,
    this.unitOfMeasure,
    this.minCaloriesPer100,
    this.maxCaloriesPer100,
    this.minProteinPer100g,
    this.maxProteinPer100g,
    this.minCarbsPer100g,
    this.maxCarbsPer100g,
    this.minFatPer100g,
    this.maxFatPer100g,
  });

  GrocerySearchFilters copyWith({
    String? searchTerm,
    GroceryCategory? category,
    UnitOfMeasureUi? unitOfMeasure,
    double? minCaloriesPer100,
    double? maxCaloriesPer100,
    double? minProteinPer100g,
    double? maxProteinPer100g,
    double? minCarbsPer100g,
    double? maxCarbsPer100g,
    double? minFatPer100g,
    double? maxFatPer100g,
  }) {
    return GrocerySearchFilters(
      searchTerm: searchTerm ?? this.searchTerm,
      category: category ?? this.category,
      unitOfMeasure: unitOfMeasure ?? this.unitOfMeasure,
      minCaloriesPer100: minCaloriesPer100 ?? this.minCaloriesPer100,
      maxCaloriesPer100: maxCaloriesPer100 ?? this.maxCaloriesPer100,
      minProteinPer100g: minProteinPer100g ?? this.minProteinPer100g,
      maxProteinPer100g: maxProteinPer100g ?? this.maxProteinPer100g,
      minCarbsPer100g: minCarbsPer100g ?? this.minCarbsPer100g,
      maxCarbsPer100g: maxCarbsPer100g ?? this.maxCarbsPer100g,
      minFatPer100g: minFatPer100g ?? this.minFatPer100g,
      maxFatPer100g: maxFatPer100g ?? this.maxFatPer100g,
    );
  }
}

class GrocerySearchException implements Exception {
  final String message;
  GrocerySearchException(this.message);

  @override
  String toString() => 'GrocerySearchException: $message';
}

class GroceryRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  GroceryRepo(this._api, this._tokenStore);

  Future<PagedResponse<Grocery>> searchGroceries({
    required GrocerySearchFilters filters,
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final query = <String, dynamic>{
        'Page': page,
        'PageSize': pageSize,
      };

      if (filters.searchTerm != null &&
          filters.searchTerm!.trim().isNotEmpty) {
        query['SearchTerm'] = filters.searchTerm!.trim();
      }

      if (filters.category != null) {
        query['Category'] = filters.category!.backendValue;
      }

      if (filters.unitOfMeasure != null) {
        query['UnitOfMeasure'] = filters.unitOfMeasure!.backendValue;
      }

      if (filters.minCaloriesPer100 != null) {
        query['MinCaloriesPer100'] =
            filters.minCaloriesPer100!.round().toString();
      }
      if (filters.maxCaloriesPer100 != null) {
        query['MaxCaloriesPer100'] =
            filters.maxCaloriesPer100!.round().toString();
      }

      void addDecimalRange(
          String minKey,
          String maxKey,
          double? min,
          double? max,
          ) {
        if (min != null) query[minKey] = min.toString();
        if (max != null) query[maxKey] = max.toString();
      }

      addDecimalRange(
        'MinProteinPer100g',
        'MaxProteinPer100g',
        filters.minProteinPer100g,
        filters.maxProteinPer100g,
      );

      addDecimalRange(
        'MinCarbsPer100g',
        'MaxCarbsPer100g',
        filters.minCarbsPer100g,
        filters.maxCarbsPer100g,
      );

      addDecimalRange(
        'MinFatPer100g',
        'MaxFatPer100g',
        filters.minFatPer100g,
        filters.maxFatPer100g,
      );

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries',
        queryParameters: query,
      );

      final data = response.data ?? {};
      return PagedResponse<Grocery>.fromJson(
        data,
            (j) => Grocery.fromJson(j),
      );
    } on ApiException catch (e) {
      throw GrocerySearchException(e.message);
    } catch (e) {
      throw GrocerySearchException('Unexpected error: $e');
    }
  }

  Future<Grocery> getByCode(String code) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries/by-code',
        queryParameters: {'code': code},
      );

      final data = response.data!;
      return Grocery.fromJson(data);

    } on ApiException catch (e) {
      throw GrocerySearchException(e.message);
    }
  }
}