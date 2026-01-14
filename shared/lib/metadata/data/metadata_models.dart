class CountrySummary {
  final String code;
  final String name;

  const CountrySummary({
    required this.code,
    required this.name,
  });

  factory CountrySummary.fromJson(Map<String, dynamic> json) {
    return CountrySummary(
      code: json['code'] as String,
      name: json['name'] as String,
    );
  }
}

class PermissionKeysResponse {
  /// Backend shape: { "permissionKeys": ["..."] }
  /// (We also accept common casing variants to be robust.)
  final List<String> permissionKeys;

  const PermissionKeysResponse({required this.permissionKeys});

  factory PermissionKeysResponse.fromJson(Map<String, dynamic> json) {
    final dynamic raw = json['permissionKeys'] ??
        json['PermissionKeys'] ??
        json['permissions'] ??
        json['Permissions'];

    final list = raw is List ? raw : const [];

    return PermissionKeysResponse(
      permissionKeys: list.whereType<String>().toList(growable: false),
    );
  }
}

class MetadataException implements Exception {
  final String message;
  MetadataException(this.message);

  @override
  String toString() => 'MetadataException: $message';
}
