import 'package:dio/dio.dart';
import 'package:cross_file/cross_file.dart';

import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';

import 'grocery_models.dart';
import 'create_grocery_request.dart';

class GrocerySearchFilters {
  final String? searchTerm;
  final GroceryCategory? category;
  final UnitOfMeasureUi? unitOfMeasure;
  final double? minCaloriesPer100;
  final double? maxCaloriesPer100;
  final double? minProteinPer100;
  final double? maxProteinPer100;
  final double? minCarbsPer100;
  final double? maxCarbsPer100;
  final double? minFatPer100;
  final double? maxFatPer100;
  final bool? isApproved;

  const GrocerySearchFilters({
    this.searchTerm,
    this.category,
    this.unitOfMeasure,
    this.minCaloriesPer100,
    this.maxCaloriesPer100,
    this.minProteinPer100,
    this.maxProteinPer100,
    this.minCarbsPer100,
    this.maxCarbsPer100,
    this.minFatPer100,
    this.maxFatPer100,
    this.isApproved
  });

  GrocerySearchFilters copyWith({
    String? searchTerm,
    GroceryCategory? category,
    UnitOfMeasureUi? unitOfMeasure,
    double? minCaloriesPer100,
    double? maxCaloriesPer100,
    double? minProteinPer100,
    double? maxProteinPer100,
    double? minCarbsPer100,
    double? maxCarbsPer100,
    double? minFatPer100,
    double? maxFatPer100,
    bool? isApproved,
  }) {
    return GrocerySearchFilters(
      searchTerm: searchTerm ?? this.searchTerm,
      category: category ?? this.category,
      unitOfMeasure: unitOfMeasure ?? this.unitOfMeasure,
      minCaloriesPer100: minCaloriesPer100 ?? this.minCaloriesPer100,
      maxCaloriesPer100: maxCaloriesPer100 ?? this.maxCaloriesPer100,
      minProteinPer100: minProteinPer100 ?? this.minProteinPer100,
      maxProteinPer100: maxProteinPer100 ?? this.maxProteinPer100,
      minCarbsPer100: minCarbsPer100 ?? this.minCarbsPer100,
      maxCarbsPer100: maxCarbsPer100 ?? this.maxCarbsPer100,
      minFatPer100: minFatPer100 ?? this.minFatPer100,
      maxFatPer100: maxFatPer100 ?? this.maxFatPer100,
      isApproved: isApproved ?? this.isApproved,
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

      if (filters.isApproved != null) {
        query['IsApproved'] = filters.isApproved!.toString();
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
        'MinProteinPer100',
        'MaxProteinPer100',
        filters.minProteinPer100,
        filters.maxProteinPer100,
      );

      addDecimalRange(
        'MinCarbsPer100',
        'MaxCarbsPer100',
        filters.minCarbsPer100,
        filters.maxCarbsPer100,
      );

      addDecimalRange(
        'MinFatPer100',
        'MaxFatPer100',
        filters.minFatPer100,
        filters.maxFatPer100,
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

  Future<PagedResponse<Grocery>> recommendedGroceries({
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries/recommended',
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
        },
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

  Future<PagedResponse<GroceryRecommendation>> enhancedRecommendedGroceries({
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries/recommended/enhanced',
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
        },
      );

      final data = response.data ?? {};
      return PagedResponse<GroceryRecommendation>.fromJson(
        data,
        (j) => GroceryRecommendation.fromJson(j),
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

  Future<void> createGrocery({
    required String name,
    required GroceryCategory category,
    required UnitOfMeasureUi unitOfMeasure,
    double? gramsPerPiece,
    required double proteinPer100,
    required double carbsPer100,
    required double fatPer100,
    required int caloriesPer100,
    String? barcode,
    XFile? imageFile,
  }) async {
    if (unitOfMeasure == UnitOfMeasureUi.piece) {
      final v = gramsPerPiece;
      if (v == null || v <= 0) {
        throw ArgumentError.value(
          gramsPerPiece,
          'gramsPerPiece',
          'Required and must be > 0 when UnitOfMeasure is Piece.',
        );
      }
    } else {
      gramsPerPiece = null;
    }

    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final map = <String, dynamic>{
      'Name': name,
      'Category': category.backendValue,
      'ProteinPer100': proteinPer100.toString(),
      'CarbsPer100': carbsPer100.toString(),
      'FatPer100': fatPer100.toString(),
      'CaloriesPer100': caloriesPer100.toString(),
      'UnitOfMeasure': unitOfMeasure.backendValue,
      if (gramsPerPiece != null) 'GramsPerPiece': gramsPerPiece.toString(),
      'Barcode': barcode,
    };

    if (imageFile != null) {
      map['Image'] = await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.name,
      );
    }

    final formData = FormData.fromMap(map);

    await _api.postMultipart<void>('/groceries', formData);
  }

  Future<Grocery> getById(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries/$id',
      );

      final data = response.data!;
      return Grocery.fromJson(data);
    } on ApiException catch (e) {
      throw GrocerySearchException(e.message);
    } catch (e) {
      throw GrocerySearchException('Unexpected error: $e');
    }
  }

  Future<void> updateGrocery({
    required String id,
    required String name,
    required GroceryCategory category,
    required UnitOfMeasureUi unitOfMeasure,
    double? gramsPerPiece,
    required double proteinPer100,
    required double carbsPer100,
    required double fatPer100,
    required int caloriesPer100,
    String? barcode,
    XFile? imageFile,
  }) async {
    if (unitOfMeasure == UnitOfMeasureUi.piece) {
      final v = gramsPerPiece;
      if (v == null || v <= 0) {
        throw ArgumentError.value(
          gramsPerPiece,
          'gramsPerPiece',
          'Required and must be > 0 when UnitOfMeasure is Piece.',
        );
      }
    } else {
      gramsPerPiece = null;
    }

    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final map = <String, dynamic>{
      'Name': name,
      'Category': category.backendValue,
      'ProteinPer100': proteinPer100.toString(),
      'CarbsPer100': carbsPer100.toString(),
      'FatPer100': fatPer100.toString(),
      'CaloriesPer100': caloriesPer100.toString(),
      'UnitOfMeasure': unitOfMeasure.backendValue,
      if (gramsPerPiece != null) 'GramsPerPiece': gramsPerPiece.toString(),
      'Barcode': barcode,
    };

    if (imageFile != null) {
      map['Image'] = await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.name,
      );
    }

    final formData = FormData.fromMap(map);

    await _api.putMultipart<void>('/groceries/$id', formData);
  }

  Future<void> deleteGrocery(String id) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    await _api.delete<void>('/groceries/$id');
  }

  Future<PagedResponse<Grocery>> listSuggestionGroceries({
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/groceries/suggestions',
        queryParameters: {
          'Page': page,
          'PageSize': pageSize,
        },
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

  Future<Grocery> suggestGrocery({
    required String name,
    required GroceryCategory category,
    required UnitOfMeasureUi unitOfMeasure,
    double? gramsPerPiece,
    required double proteinPer100,
    required double carbsPer100,
    required double fatPer100,
    required int caloriesPer100,
    String? barcode,
    XFile? imageFile,
  }) async {
    return suggestGroceryRequest(
      CreateGroceryRequest(
        name: name,
        category: category,
        unitOfMeasure: unitOfMeasure,
        gramsPerPiece: gramsPerPiece,
        proteinPer100: proteinPer100,
        carbsPer100: carbsPer100,
        fatPer100: fatPer100,
        caloriesPer100: caloriesPer100,
        barcode: barcode,
        imageFile: imageFile,
      ),
    );
  }

  Future<Grocery> suggestGroceryRequest(CreateGroceryRequest request) async {
    try {
      // Keep the same validation rules as createGrocery/updateGrocery.
      if (request.unitOfMeasure == UnitOfMeasureUi.piece) {
        final v = request.gramsPerPiece;
        if (v == null || v <= 0) {
          throw ArgumentError.value(
            request.gramsPerPiece,
            'gramsPerPiece',
            'Required and must be > 0 when UnitOfMeasure is Piece.',
          );
        }
      }

      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final map = <String, dynamic>{
        'Name': request.name,
        'Category': request.category.backendValue,
        'ProteinPer100': request.proteinPer100.toString(),
        'CarbsPer100': request.carbsPer100.toString(),
        'FatPer100': request.fatPer100.toString(),
        'CaloriesPer100': request.caloriesPer100.toString(),
        'UnitOfMeasure': request.unitOfMeasure.backendValue,
        if (request.unitOfMeasure == UnitOfMeasureUi.piece &&
            request.gramsPerPiece != null)
          'GramsPerPiece': request.gramsPerPiece.toString(),
        'Barcode': request.barcode,
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
        '/groceries/suggestions',
        formData,
      );

      return Grocery.fromJson(response);
    } on ApiException catch (e) {
      throw GrocerySearchException(e.message);
    } catch (e) {
      throw GrocerySearchException('Unexpected error: $e');
    }
  }

  Future<void> approveSuggestion(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.post<void>('/groceries/suggestions/$id/approve');
    } on ApiException catch (e) {
      throw GrocerySearchException(e.message);
    } catch (e) {
      throw GrocerySearchException('Unexpected error: $e');
    }
  }

}
