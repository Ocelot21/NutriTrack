import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';

/// Key under which the raw JWT access token is stored.
const _tokenStorageKey = 'auth_token';

/// Simple storage for the raw access token using [SharedPreferences].
///
/// This class is deliberately small and framework-agnostic so it can be used
/// both by the mobile and desktop apps.
class TokenStore {
  const TokenStore();

  /// Saves the raw JWT token as a string.
  Future<void> save(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenStorageKey, token);
  }

  /// Reads the raw JWT token string, or `null` if not present.
  ///
  /// This keeps the old behaviour so existing code that only needs the
  /// token for Authorization headers continues to work.
  Future<String?> read() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_tokenStorageKey);
  }

  /// Removes the stored token.
  Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenStorageKey);
  }

  /// Reads and parses the current token into [JwtPayload], if available
  /// and structurally valid.
  Future<JwtPayload?> readParsed() async {
    final raw = await read();
    if (raw == null || raw.isEmpty) return null;
    return JwtPayload.tryParse(raw);
  }

  /// Convenience: checks if current token grants a specific permission.
  ///
  /// This is **only for controlling the UI**. All real authorization
  /// decisions must still be enforced on the backend.
  Future<bool> hasPermission(String permission) async {
    final payload = await readParsed();
    if (payload == null) return false;
    return payload.hasPermission(permission);
  }

  /// Convenience: checks if current token has any of the given permissions.
  Future<bool> hasAnyPermission(Iterable<String> permissions) async {
    final payload = await readParsed();
    if (payload == null) return false;
    return payload.hasAnyPermission(permissions);
  }
}

/// Decoded JWT payload with common NutriTrack claims.
class JwtPayload {
  final String? subject;
  final String? email;
  final String? givenName;
  final String? familyName;
  final String? userName;
  final String? role;
  final List<String> permissions;
  final DateTime? expiresAt;
  final Map<String, dynamic> raw; // full payload

  const JwtPayload({
    this.subject,
    this.email,
    this.givenName,
    this.familyName,
    this.userName,
    this.role,
    this.permissions = const [],
    this.expiresAt,
    this.raw = const {},
  });

  /// Parses a JWT string and returns [JwtPayload] if successful,
  /// or `null` if the token is malformed.
  static JwtPayload? tryParse(String token) {
    try {
      final parts = token.split('.');
      if (parts.length < 2) {
        return null;
      }

      final payloadPart = parts[1];

      // JWT uses base64url without padding. Normalise it.
      final normalized = base64Url.normalize(payloadPart);
      final decodedBytes = base64Url.decode(normalized);
      final jsonString = utf8.decode(decodedBytes);
      final dynamic jsonMap = json.decode(jsonString);

      if (jsonMap is! Map<String, dynamic>) {
        return null;
      }

      final map = jsonMap;

      String? _string(dynamic v) => v == null ? null : v.toString();

      final subject = _string(map['sub']);
      final email = _string(map['email']);
      final givenName = _string(map['given_name']);
      final familyName = _string(map['family_name']);
      final userName =
      _string(map['unique_name'] ?? map['username'] ?? map['name']);
      final role = _string(map['role']);

      // perms claim can be a list or a single string.
      final permsRaw = map['perms'];
      final permissions = <String>[];
      if (permsRaw is List) {
        permissions.addAll(permsRaw
            .where((e) => e != null)
            .map((e) => e.toString())
            .where((e) => e.isNotEmpty));
      } else if (permsRaw is String && permsRaw.isNotEmpty) {
        permissions.add(permsRaw);
      }

      // exp is seconds since epoch (Unix time)
      DateTime? expiresAt;
      final expRaw = map['exp'];
      if (expRaw is int) {
        expiresAt =
            DateTime.fromMillisecondsSinceEpoch(expRaw * 1000, isUtc: true)
                .toLocal();
      } else if (expRaw is num) {
        expiresAt =
            DateTime.fromMillisecondsSinceEpoch(expRaw.toInt() * 1000,
                isUtc: true)
                .toLocal();
      }

      return JwtPayload(
        subject: subject,
        email: email,
        givenName: givenName,
        familyName: familyName,
        userName: userName,
        role: role,
        permissions: permissions,
        expiresAt: expiresAt,
        raw: map,
      );
    } catch (_) {
      // If anything goes wrong while decoding, just treat as invalid token.
      return null;
    }
  }

  /// Whether the token has an expiry time and it has passed.
  bool get isExpired {
    if (expiresAt == null) return false;
    return DateTime.now().isAfter(expiresAt!);
  }

  /// Checks whether the token is about to expire within [threshold].
  bool isExpiringSoon([Duration threshold = const Duration(minutes: 5)]) {
    if (expiresAt == null) return false;
    return expiresAt!.isBefore(DateTime.now().add(threshold));
  }

  /// Simple helpers for permission / role checks.
  bool hasPermission(String permission) => permissions.contains(permission);

  bool hasAnyPermission(Iterable<String> perms) =>
      perms.any(permissions.contains);

  bool get isAdmin =>
      role != null &&
          (role!.toLowerCase() == 'admin' || role!.toLowerCase() == 'administrator');
}