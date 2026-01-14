import '../../core/api_client.dart';
import '../../core/api_exception.dart';
import 'metadata_models.dart';

class MetadataRepo {
  final ApiClient _api;

  MetadataRepo(this._api);

  List<CountrySummary>? _cachedCountries;
  List<String>? _cachedTimeZones;
  List<String>? _cachedPermissionKeys;

  Future<List<CountrySummary>> getCountries({bool forceRefresh = false}) async {
    if (!forceRefresh && _cachedCountries != null) {
      return _cachedCountries!;
    }

    try {
      final response = await _api.get<Map<String, dynamic>>(
        '/metadata/countries',
      );

      final data = response.data ?? {};
      final countriesJson = data['countries'] as List<dynamic>? ?? const [];

      final countries = countriesJson
          .whereType<Map<String, dynamic>>()
          .map(CountrySummary.fromJson)
          .toList(growable: false);

      _cachedCountries = countries;
      return countries;
    } on ApiException catch (e) {
      throw MetadataException(e.message);
    } catch (e) {
      throw MetadataException('Unexpected error: $e');
    }
  }

  Future<List<String>> getTimeZones({bool forceRefresh = false}) async {
    if (!forceRefresh && _cachedTimeZones != null) {
      return _cachedTimeZones!;
    }

    try {
      final response = await _api.get<Map<String, dynamic>>(
        '/metadata/time-zones',
      );

      final data = response.data ?? {};

      final dynamic raw = data['timeZones'] ?? data['TimeZones'] ?? data;

      List<dynamic> list;
      if (raw is List) {
        list = raw;
      } else {
        list = const [];
      }

      final timeZones = list
          .whereType<String>()
          .toList(growable: false);

      _cachedTimeZones = timeZones;
      return timeZones;
    } on ApiException catch (e) {
      throw MetadataException(e.message);
    } catch (e) {
      throw MetadataException('Unexpected error: $e');
    }
  }

  Future<List<String>> getPermissionKeys({bool forceRefresh = false}) async {
    if (!forceRefresh && _cachedPermissionKeys != null) {
      return _cachedPermissionKeys!;
    }

    try {
      final response = await _api.get<Map<String, dynamic>>(
        '/metadata/permission-keys',
      );

      final data = response.data ?? const <String, dynamic>{};
      final parsed = PermissionKeysResponse.fromJson(data);

      _cachedPermissionKeys = parsed.permissionKeys;
      return parsed.permissionKeys;
    } on ApiException catch (e) {
      throw MetadataException(e.message);
    } catch (e) {
      throw MetadataException('Unexpected error: $e');
    }
  }
}
