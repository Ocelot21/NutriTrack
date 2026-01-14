import 'package:dio/dio.dart';
import 'package:nutritrack_mobile/features/user/data/two_factor_models.dart';

import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'user_models.dart';

class UserRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  UserRepo(this._api, this._tokenStore);

  Future<Me> getMe() async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final response = await _api.get<Map<String, dynamic>>('/me');
    return Me.fromJson(response.data!);
  }

 Future<void> updateProfileFields(Map<String, dynamic> fields) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    await _api.patch<void>(
      '/me',
      data: fields,
      options: Options(contentType: Headers.jsonContentType),
    );
  }

  Future<void> uploadAvatar(MultipartFile file) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final formData = FormData.fromMap({'Avatar': file});

    await _api.postMultipart<void>('/me/avatar', formData);
  }

  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    await _api.put<void>(
      '/me/change-password',
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  Future<TotpSetupData> setupTotp() async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    final response = await _api.post<Map<String, dynamic>>('/auth/totp/setup');
    return TotpSetupData.fromJson(response.data!);
  }

  Future<void> confirmTotp(String code) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    await _api.post<void>(
      '/auth/totp/confirm',
      data: {
        'code': code
      },
    );
  }

  Future<void> disableTotp(String code) async {
    final token = await _tokenStore.read();
    _api.setAuthToken(token);

    await _api.post<void>(
      '/auth/totp/disable',
      data: {
        'code': code
      },
    );
  }

}