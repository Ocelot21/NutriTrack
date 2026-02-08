import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'health_profile_request.dart';

class HealthProfileException implements Exception {
  final String message;
  HealthProfileException(this.message);

  @override
  String toString() => 'HealthProfileException: $message';
}

class HealthProfileRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  HealthProfileRepo(this._api, this._tokenStore);

  Future<void> upsertHealthProfile(HealthProfileRequest request) async {
    try {
      final token = await _tokenStore.read();
      //print('HEALTH PROFILE → token from store: $token');

      _api.setAuthToken(token);
      //print('HEALTH PROFILE → calling PUT /me/health-profile with headers: ${_api.raw.options.headers}');

      await _api.put<Map<String, dynamic>>(
        '/me/health-profile',
        data: request.toJson(),
      );
    } on ApiException catch (e) {

      throw HealthProfileException(e.message);
    } catch (e) {
      throw HealthProfileException('Unexpected error: $e');
    }
  }
}
