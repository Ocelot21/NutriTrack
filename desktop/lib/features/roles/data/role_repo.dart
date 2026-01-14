import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import 'role_models.dart';

class RolesException implements Exception {
  final String message;
  RolesException(this.message);

  @override
  String toString() => 'RolesException: $message';
}

class RoleRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  RoleRepo(this._api, this._tokenStore);

  /// GET /api/admin/roles
  Future<RolesListResponse> listRoles() async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>('/admin/roles');
      final data = response.data ?? const <String, dynamic>{};
      return RolesListResponse.fromJson(data);
    } on ApiException catch (e) {
      throw RolesException(e.message);
    } catch (e) {
      throw RolesException('Unexpected error: $e');
    }
  }

  /// GET /api/admin/roles/{id}
  Future<RoleResponse> getRoleById(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>('/admin/roles/$id');
      final data = response.data ?? const <String, dynamic>{};
      return RoleResponse.fromJson(data);
    } on ApiException catch (e) {
      throw RolesException(e.message);
    } catch (e) {
      throw RolesException('Unexpected error: $e');
    }
  }

  /// POST /api/admin/roles
  Future<RoleResponse> createRole(CreateRoleRequest request) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.post<Map<String, dynamic>>(
        '/admin/roles',
        data: request.toJson(),
      );

      final data = response.data ?? const <String, dynamic>{};
      return RoleResponse.fromJson(data);
    } on ApiException catch (e) {
      throw RolesException(e.message);
    } catch (e) {
      throw RolesException('Unexpected error: $e');
    }
  }

  /// PATCH /api/admin/roles/{id}/permissions
  Future<void> updateRolePermissions({
    required String roleId,
    required List<String>? add,
    required List<String>? remove,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.patch<void>(
        '/admin/roles/$roleId/permissions',
        data: UpdateRolePermissionsRequest(add: add, remove: remove).toJson(),
      );
    } on ApiException catch (e) {
      throw RolesException(e.message);
    } catch (e) {
      throw RolesException('Unexpected error: $e');
    }
  }

  /// DELETE /api/admin/roles/{id}
  Future<void> deleteRole(String id) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      await _api.delete<void>('/admin/roles/$id');
    } on ApiException catch (e) {
      throw RolesException(e.message);
    } catch (e) {
      throw RolesException('Unexpected error: $e');
    }
  }
}

