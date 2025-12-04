import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../features/auth/presentation/login_page.dart';
import '../features/auth/presentation/register_page.dart';
import '../features/health_profile/presentation/health_profile_page.dart';
import '../features/home/presentation/home_page.dart';

enum AppRoute {
  login,
  register,
  home,
  healthProfile,
}

class AppRouter {
  AppRouter._();

  static final GoRouter router = GoRouter(
    initialLocation: '/login',
    routes: [
      GoRoute(
        path: '/login',
        name: AppRoute.login.name,
        pageBuilder: (context, state) =>
            const MaterialPage(child: LoginPage()),
      ),
      GoRoute(
        path: '/register',
        name: AppRoute.register.name,
        pageBuilder: (context, state) =>
            const MaterialPage(child: RegisterPage()),
      ),
      GoRoute(
        path: '/home',
        name: AppRoute.home.name,
        pageBuilder: (context, state) =>
            const MaterialPage(child: HomePage()),
      ),
      GoRoute(
        path: '/health-profile',
        name: AppRoute.healthProfile.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: HealthProfilePage()),
      ),
    ],
  );
}
