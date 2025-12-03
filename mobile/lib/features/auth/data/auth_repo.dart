import 'package:dio/dio.dart';
import 'package:nutritrack_mobile/features/auth/data/register_request.dart';

import '../../../core/api_client.dart';
import '../../../core/token_store.dart';
import '../../../core/api_exception.dart';

class AuthException implements Exception {
  final String message;
  AuthException(this.message);

  @override
  String toString() => 'AuthException: $message';
}

class AuthRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  AuthRepo(this._api, this._tokenStore);

  String? _extractToken(dynamic data) {
    if (data == null) return null;

    Map<String, dynamic>? map;
    if (data is Map<String, dynamic>) {
      map = data;
    } else if (data is Map) {
      map = Map<String, dynamic>.from(data);
    }

    if (map == null) return null;

    final lower = <String, dynamic>{
      for (final e in map.entries) e.key.toString().toLowerCase(): e.value,
    };

    final candidates = [
      'token',
      'access_token',
      'accessToken'.toLowerCase(),
      'jwt',
    ];

    for (final key in candidates) {
      final value = lower[key];
      if (value is String && value.isNotEmpty) return value;
    }

    final dataField = lower['data'];
    if (dataField != null && dataField is Map) {
      return _extractToken(dataField);
    }

    return null;
  }

  Future<bool> login({
    required String email,
    required String password,
  }) async {
    try {
      final response = await _api.post<Map<String, dynamic>>(
        '/auth/login',
        data: {
          'email': email,
          'password': password,
        },
      );

      final token = _extractToken(response.data);
      if (token != null && token.isNotEmpty) {
        await _tokenStore.save(token);
        return true;
      }

      throw AuthException('Login succeeded but token was missing in response.');
    } on ApiException catch (e) {
      final codes = e.problem?.errorKeys ?? const [];
      if (codes.contains('Authentication.InvalidCredentials')) {
        throw AuthException('The provided email or password is incorrect.');
      }

      throw AuthException(e.message);
    } catch (e) {
      throw AuthException('Unexpected error: $e');
    }
  }

  Future<void> logout() => _tokenStore.clear();

  Future<bool> isLoggedIn() async {
    final t = await _tokenStore.read();
    return t != null && t.isNotEmpty;
  }

  Future<void> register(RegisterRequest request) async {
    try {
      await _api.post<Map<String, dynamic>>(
        '/auth/register',
        data: request.toJson(),
      );
    } on ApiException catch (e) {
      throw AuthException(e.message);
    } catch (e) {
      throw AuthException('Unexpected error: $e');
    }
  }

}
