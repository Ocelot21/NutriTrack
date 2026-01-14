import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/login_page.dart';
import '../../features/auth/presentation/two_factor_page.dart';
import '../../features/dashboard/presentation/dashboard_page.dart';
import '../../features/groceries/presentation/groceries_admin_page.dart';
import '../../features/exercises/presentation/exercises_admin_page.dart';
import '../../features/suggestions/presentation/suggestions_page.dart';
import '../../features/users/presentation/users_page.dart';
import '../features/exercises/presentation/exercise_details_page.dart';
import '../features/groceries/presentation/grocery_details_page.dart';
import '../../features/roles/presentation/roles_page.dart';
import '../../features/reports/presentation/reports_page.dart';
import '../../features/auth/presentation/unauthorized_page.dart';
import '../core/providers.dart';
import '../features/auth/presentation/logout_helpers.dart';
import 'router_refresh_listenable.dart';

final GlobalKey<NavigatorState> routerKey = GlobalKey<NavigatorState>();
final RouterRefreshListenable _routerRefresh = RouterRefreshListenable();

/// Call this after login/logout to force GoRouter to re-run redirect logic.
void pingRouterRefresh() => _routerRefresh.ping();

final GoRouter router = GoRouter(
  navigatorKey: routerKey,
  initialLocation: '/login',
  refreshListenable: _routerRefresh,
  routes: [
    GoRoute(
      path: '/login',
      builder: (context, state) => const LoginPage(),
    ),
    GoRoute(
      path: '/two-factor',
      builder: (context, state) => const TwoFactorPage(),
    ),
    GoRoute(
      path: '/unauthorized',
      builder: (context, state) => const UnauthorizedPage(),
    ),
    ShellRoute(
      builder: (context, state, child) {
        final location = state.uri.toString();
        return AdminShell(
          location: location,
          child: child,
        );
      },
      routes: [
        GoRoute(
          path: '/',
          builder: (context, state) => const DashboardPage(),
        ),
        GoRoute(
          path: '/groceries',
          builder: (context, state) => const GroceriesAdminPage(),
        ),
        GoRoute(
          path: '/groceries/:id',
          builder: (context, state) {
            final id = state.pathParameters['id']!;
            return GroceryDetailsPage(id: id);
          },
        ),
        GoRoute(
          path: '/exercises',
          builder: (context, state) => const ExercisesAdminPage(),
        ),
        GoRoute(
          path: '/exercises/:id',
          builder: (context, state) {
            final id = state.pathParameters['id']!;
            return ExerciseDetailsPage(id: id);
          },
        ),
        GoRoute(
          path: '/suggestions',
          builder: (context, state) => const SuggestionsPage(),
        ),
        GoRoute(
          path: '/users',
          builder: (context, state) => const UsersPage(),
        ),
        GoRoute(
          path: '/roles',
          builder: (context, state) => const RolesPage(),
        ),
        GoRoute(
          path: '/reports',
          builder: (context, state) => const ReportsPage(),
        ),
      ],
    ),
  ],
  redirect: (context, state) {
    final container = ProviderScope.containerOf(context, listen: false);

    final isLoginRoute = state.matchedLocation == '/login';
    final isTwoFactorRoute = state.matchedLocation == '/two-factor';
    final isUnauthorizedRoute = state.matchedLocation == '/unauthorized';

    final jwtAsync = container.read(jwtPayloadProvider);
    if (jwtAsync.isLoading) return null;

    final jwt = jwtAsync.maybeWhen(
      data: (v) => v,
      orElse: () => null,
    );

    final isAuthed = jwt != null;

    // Not authenticated: only allow login/2fa pages.
    if (!isAuthed) {
      return (isLoginRoute || isTwoFactorRoute) ? null : '/login';
    }

    final isAdmin = jwt.isAdmin;

    // Authenticated but not admin: only allow unauthorized page.
    if (!isAdmin) {
      return isUnauthorizedRoute ? null : '/unauthorized';
    }

    // Authenticated admin: keep away from auth/unauthorized pages.
    if (isLoginRoute || isTwoFactorRoute || isUnauthorizedRoute) return '/';

    return null;
  },
);

class AdminShell extends ConsumerWidget {
  final Widget child;
  final String location;

  const AdminShell({
    super.key,
    required this.child,
    required this.location,
  });

  int _selectedIndex() {
    if (location.startsWith('/groceries')) return 1;
    if (location.startsWith('/exercises')) return 2;
    if (location.startsWith('/suggestions')) return 3;
    if (location.startsWith('/users')) return 4;
    if (location.startsWith('/roles')) return 5;
    if (location.startsWith('/reports')) return 6;

    // dashboard
    return 0;
  }

  void _onDestinationSelected(BuildContext context, int index) {
    switch (index) {
      case 0:
        context.go('/');
        break;
      case 1:
        context.go('/groceries');
        break;
      case 2:
        context.go('/exercises');
        break;
      case 3:
        context.go('/suggestions');
        break;
      case 4:
        context.go('/users');
        break;
      case 5:
        context.go('/roles');
        break;
      case 6:
        context.go('/reports');
        break;
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final selected = _selectedIndex();

    return Scaffold(
      body: Row(
        children: [
          SizedBox(
            child: Container(
              color: theme.colorScheme.surfaceContainerLowest,
              child: Column(
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(16, 24, 16, 24),
                    child: Center(
                      child: Image.asset(
                        'assets/logo/logo_horizontal_1200_400.png',
                        package: 'nutritrack_shared',
                        height: 60,
                        fit: BoxFit.contain,
                      ),
                    ),
                  ),

                  // NAVIGATION
                  Expanded(
                    child: NavigationRail(
                      backgroundColor: Colors.transparent,
                      selectedIndex: selected,
                      onDestinationSelected: (i) =>
                          _onDestinationSelected(context, i),
                      labelType: NavigationRailLabelType.all,
                      minWidth: 72,
                      destinations: const [
                        NavigationRailDestination(
                          icon: Icon(Icons.dashboard_outlined),
                          selectedIcon: Icon(Icons.dashboard),
                          label: Text('Dashboard'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.restaurant_outlined),
                          selectedIcon: Icon(Icons.restaurant),
                          label: Text('Groceries'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.fitness_center_outlined),
                          selectedIcon: Icon(Icons.fitness_center),
                          label: Text('Exercises'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.inbox_outlined),
                          selectedIcon: Icon(Icons.inbox),
                          label: Text('Suggestions'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.people_outline),
                          selectedIcon: Icon(Icons.people),
                          label: Text('Users'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.admin_panel_settings_outlined),
                          selectedIcon: Icon(Icons.admin_panel_settings),
                          label: Text('Roles'),
                        ),
                        NavigationRailDestination(
                          icon: Icon(Icons.assessment_outlined),
                          selectedIcon: Icon(Icons.assessment),
                          label: Text('Reports'),
                        ),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(bottom: 24, top: 8),
                    child: IconButton(
                      tooltip: 'Logout',
                      icon: const Icon(Icons.logout),
                      onPressed: () async {
                        await logoutAndGoToLogin(ref);
                      },
                    ),
                  ),
                ],
              ),
            ),
          ),

          const VerticalDivider(width: 1),

          // CONTENT
          Expanded(
            child: Container(
              color: theme.colorScheme.surface,
              child: child,
            ),
          ),
        ],
      ),
    );
  }
}
