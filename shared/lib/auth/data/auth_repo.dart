import '../../../../core/api_client.dart';
import '../../../../core/token_store.dart';
import '../../../../core/api_exception.dart';
import 'register_request.dart';
import 'login_result.dart';

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

  Future<LoginResult> login({
    required String emailOrUsername,
    required String password,
  }) async {
    try {
      final response = await _api.post<Map<String, dynamic>>(
        '/auth/login',
        data: {
          'emailOrUsername': emailOrUsername,
          'password': password,
        },
      );

      final data = response.data ?? {};

      final token = data['accessToken'] as String?;
      final requiresTwoFactor = data['requiresTwoFactor'] as bool? ?? false;
      final challengeId = data['twoFactorChallengeId'] as String?;

      if (token != null && token.isNotEmpty) {
        await _tokenStore.save(token);
        _api.setAuthToken(token);
        return LoginAuthenticated(token);
      }

      if (requiresTwoFactor) {
        if (challengeId == null || challengeId.isEmpty) {
          throw AuthException('Login requires 2FA but challenge id is missing.');
        }
        return LoginTwoFactorRequired(challengeId);
      }

      throw AuthException('Invalid login response (missing accessToken / 2FA flags).');
    } on ApiException catch (e) {
      throw AuthException(e.message);
    } catch (e) {
      throw AuthException('Unexpected error: $e');
    }
  }

  Future<void> verifyTwoFactor({
    required String challengeId,
    required String code,
  }) async {
    try {
      final response = await _api.post<Map<String, dynamic>>(
        '/auth/login/2fa',
        data: {
          'challengeId': challengeId,
          'code': code,
        },
      );

      final data = response.data ?? {};
      final token = data['accessToken'] as String?;

      if (token == null || token.isEmpty) {
        throw AuthException('2FA succeeded but accessToken is missing in response.');
      }

      await _tokenStore.save(token);
      _api.setAuthToken(token);
    } on ApiException catch (e) {
      throw AuthException(e.message);
    } catch (e) {
      throw AuthException('Unexpected error: $e');
    }
  }

  Future<void> logout() async {
    await _tokenStore.clear();
    // Also clear in-memory auth header to avoid sending stale tokens.
    _api.setAuthToken('');
  }

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