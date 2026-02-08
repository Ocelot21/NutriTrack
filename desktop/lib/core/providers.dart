import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import 'package:nutritrack_shared/nutritrack_shared.dart';

/// API base URL.
///
/// You can override this at runtime:
/// flutter run --dart-define=API_BASE_URL=http://localhost:5000/api
///
/// If not provided, we fall back to a sensible default.
final baseUrlProvider = Provider<String>((ref) {
  const fallback = 'http://localhost:5000/api';

  const defined = String.fromEnvironment('API_BASE_URL', defaultValue: '');
  final v = defined.trim();
  if (v.isEmpty) return fallback;

  // Basic sanity: ensure scheme exists so Dio doesn't crash.
  if (!v.startsWith('http://') && !v.startsWith('https://')) {
    debugPrint(
      'Invalid API_BASE_URL (missing http/https): "$v". Falling back to $fallback',
    );
    return fallback;
  }

  return v;
});


/// ---------------------------------------------------------------------------
/// DIO CONFIG
/// ---------------------------------------------------------------------------
final dioProvider = Provider<Dio>((ref) {
  final baseUrl = ref.watch(baseUrlProvider);

  return Dio(
    BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      headers: const {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ),
  );
});


/// ---------------------------------------------------------------------------
/// API CLIENT
/// ---------------------------------------------------------------------------
/// We attach token on startup automatically.
final apiClientProvider = Provider<ApiClient>((ref) {
  final dio = ref.watch(dioProvider);
  final client = ApiClient(dio);

  // Load token on startup if exists (fire-and-forget). Repos also set token per-call.
  final tokenStore = ref.watch(tokenStoreProvider);
  tokenStore.read().then((token) {
    if (token != null && token.isNotEmpty) {
      client.setAuthToken(token);
    }
  });

  return client;
});

/// Async variant that guarantees the auth token (if any) is applied
/// before the client is used.
final apiClientReadyProvider = FutureProvider<ApiClient>((ref) async {
  final dio = ref.watch(dioProvider);
  final client = ApiClient(dio);

  final tokenStore = ref.watch(tokenStoreProvider);
  final token = await tokenStore.read();
  if (token != null && token.isNotEmpty) {
    client.setAuthToken(token);
  }

  return client;
});


/// ---------------------------------------------------------------------------
/// TOKEN STORE
/// ---------------------------------------------------------------------------
final tokenStoreProvider = Provider<TokenStore>((ref) {
  return const TokenStore();
});


/// ---------------------------------------------------------------------------
/// WATCHES THE RAW TOKEN STRING SO AUTH STATE UPDATES PROPAGATE THROUGH RIVERPOD
/// ---------------------------------------------------------------------------
final authTokenProvider = FutureProvider<String?>((ref) async {
  final store = ref.watch(tokenStoreProvider);
  return store.read();
});


/// ---------------------------------------------------------------------------
/// DECODED JWT PAYLOAD PROVIDER
/// (THIS ALLOWS THE WHOLE UI TO KNOW WHO IS LOGGED IN.)
/// ---------------------------------------------------------------------------
final jwtPayloadProvider = FutureProvider<JwtPayload?>((ref) async {
  // Depend on token value so this provider recomputes when token changes.
  final token = await ref.watch(authTokenProvider.future);
  if (token == null || token.isEmpty) return null;
  // Parse directly from token; avoids stale caching.
  return JwtPayload.tryParse(token);
});


/// ---------------------------------------------------------------------------
/// HANDY PROVIDERS FOR PERMISSION CHECKING
/// ---------------------------------------------------------------------------

/// Returns true/false depending on whether the current token has a given permission.
final userHasPermissionProvider =
FutureProvider.family<bool, String>((ref, permission) async {
  final store = ref.watch(tokenStoreProvider);
  return await store.hasPermission(permission);
});

/// Returns whether the user is admin.
final isAdminProvider = FutureProvider<bool>((ref) async {
  final payload = await ref.watch(jwtPayloadProvider.future);
  return payload?.isAdmin ?? false;
});
