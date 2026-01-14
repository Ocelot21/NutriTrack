import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../app/router.dart';
import '../../../core/providers.dart';
import 'auth_providers.dart';

/// Logs out the user and guarantees the UI moves to /login.
///
/// Why this exists:
/// - logout clears SharedPreferences but providers may still have cached values
/// - GoRouter redirect won't rerun unless navigation happens
///
/// This helper:
/// - clears token + in-memory header via AuthRepo.logout
/// - invalidates auth-related providers
/// - navigates using the global router (no BuildContext required)
Future<void> logoutAndGoToLogin(WidgetRef ref) async {
  await ref.read(authRepoProvider).logout();

  // Ensure auth state recomputes immediately.
  ref.invalidate(authTokenProvider);
  ref.invalidate(jwtPayloadProvider);
  ref.invalidate(isAdminProvider);

  // Also reset any transient auth UI state (like 2FA challenge).
  ref.read(authControllerProvider.notifier).resetTwoFactor();

  // Trigger router redirects.
  pingRouterRefresh();

  // Use the global router so this works even if context is stale.
  router.go('/login');
}
