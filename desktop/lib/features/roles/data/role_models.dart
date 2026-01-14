class RoleResponse {
  final String id;
  final String name;
  final String? description;
  final List<String> permissions;

  const RoleResponse({
    required this.id,
    required this.name,
    required this.description,
    required this.permissions,
  });

  factory RoleResponse.fromJson(Map<String, dynamic> json) {
    return RoleResponse(
      id: json['id'] as String,
      name: json['name'] as String,
      description: json['description'] as String?,
      permissions: (json['permissions'] as List<dynamic>? ?? const [])
          .map((e) => e.toString())
          .toList(growable: false),
    );
  }
}

class RolesListResponse {
  final List<RoleResponse> roles;

  const RolesListResponse({required this.roles});

  factory RolesListResponse.fromJson(Map<String, dynamic> json) {
    return RolesListResponse(
      roles: (json['roles'] as List<dynamic>? ?? const [])
          .whereType<Map<String, dynamic>>()
          .map(RoleResponse.fromJson)
          .toList(growable: false),
    );
  }
}

class CreateRoleRequest {
  final String name;
  final String? description;
  final List<String>? permissions;

  const CreateRoleRequest({
    required this.name,
    this.description,
    this.permissions,
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'description': description,
        'permissions': permissions,
      };
}

class UpdateRolePermissionsRequest {
  final List<String>? add;
  final List<String>? remove;

  const UpdateRolePermissionsRequest({
    this.add,
    this.remove,
  });

  Map<String, dynamic> toJson() => {
        'add': add,
        'remove': remove,
      };
}

