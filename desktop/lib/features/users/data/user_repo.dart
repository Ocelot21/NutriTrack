import 'package:dio/dio.dart';

import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';
import 'user_models.dart';

class UserRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  UserRepo(this._api, this._tokenStore);

  // --- Me endpoints ---
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

    await _api.post<void>(
      '/me/change-password',
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  // --- Admin users endpoints (UsersController: /api/admin/users) ---

  Future<PagedResponse<UserResponse>> listUsers({
    int pageNumber = 1,
    int pageSize = 20,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/admin/users',
        queryParameters: {
          'pageNumber': pageNumber,
          'pageSize': pageSize,
        },
      );

      final data = response.data ?? {};
      return PagedResponse<UserResponse>.fromJson(
        data,
        (j) => UserResponse.fromJson(j),
      );
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<UserResponse> getUserById(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/admin/users/$id',
      );

      final data = response.data ?? {};
      return UserResponse.fromJson(data);
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<void> deleteUserById(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>('/admin/users/$id');
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<void> setUserRole({
    required String userId,
    required String roleId,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.put<void>(
        '/admin/users/$userId/role',
        data: SetUserRoleRequest(roleId).toJson(),
      );
    } on ApiException catch (e) {
      throw Exception(e.message);
    } catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }
}