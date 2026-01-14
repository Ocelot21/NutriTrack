import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/role_repo.dart';
import '../data/role_models.dart';

import 'package:nutritrack_shared/metadata/data/metadata_repo.dart';

final roleRepoProvider = Provider<RoleRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return RoleRepo(api, store);
});

final metadataRepoProvider = Provider<MetadataRepo>((ref) {
  final api = ref.read(apiClientProvider);
  return MetadataRepo(api);
});

final permissionKeysProvider = FutureProvider<List<String>>((ref) async {
  final repo = ref.read(metadataRepoProvider);
  return repo.getPermissionKeys();
});

final rolesListProvider = FutureProvider<List<RoleResponse>>((ref) async {
  final repo = ref.read(roleRepoProvider);
  final resp = await repo.listRoles();
  return resp.roles;
});

final roleDetailsProvider = FutureProvider.autoDispose.family<RoleResponse, String>(
  (ref, roleId) async {
    final repo = ref.read(roleRepoProvider);
    return repo.getRoleById(roleId);
  },
);
