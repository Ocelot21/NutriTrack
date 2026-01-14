import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:nutritrack_mobile/features/exercises/presentation/exercise_suggest_page.dart';
import 'package:nutritrack_mobile/features/exercises/presentation/exercises_menu_page.dart';
import 'package:nutritrack_mobile/features/groceries/presentation/groceries_menu_page.dart';
import 'package:nutritrack_mobile/features/social/presentation/social_page.dart';

import '../features/achievements/presentation/user_achievements_page.dart';
import '../features/auth/presentation/login_page.dart';
import '../features/auth/presentation/register_page.dart';
import '../features/auth/presentation/two_factor_page.dart';
import '../features/exercises/presentation/edit_exercise_log_page.dart';
import '../features/exercises/presentation/exercise_log_page.dart';
import '../features/exercises/presentation/exercise_search_page.dart';
import '../features/groceries/presentation/grocery_suggest_page.dart';
import '../features/groceries/presentation/grocery_search_page.dart';
import '../features/health_profile/presentation/health_profile_page.dart';
import '../features/home/presentation/home_page.dart';
import '../features/meals/presentation/create_meal_item_page.dart';
import '../features/meals/presentation/create_meal_page.dart';
import '../features/meals/presentation/edit_meal_item_page.dart';
import '../features/meals/presentation/edit_meal_page.dart';
import '../features/home/data/daily_overview_models.dart';
import '../features/notifications/presentation/notifications_page.dart';
import '../features/scanner/scanner_page.dart';
import '../features/user/presentation/profile_page.dart';
import '../features/user/presentation/two_factor_settings_page.dart';
import '../features/user_goals/presentation/user_goal_history_page.dart';
import '../features/user_goals/presentation/user_goal_page.dart';
import '../features/groceries/presentation/recommended_groceries_page.dart';
import '../features/social/presentation/social_profile_page.dart';

enum AppRoute {
  login,
  register,
  home,
  userProfile,
  healthProfile,
  createMeal,
  editMeal,
  exercises,
  userGoal,
  userGoalHistory,
  notifications,
  achievements,
  groceriesMenu,
  exercisesMenu,
  exercisesSuggest,
  grocerySuggest,
  twoFactorSettings,
  twoFactor,
  social
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
        path: '/profile',
        name: AppRoute.userProfile.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: ProfilePage()),
      ),
      GoRoute(
        path: '/health-profile',
        name: AppRoute.healthProfile.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: HealthProfilePage()),
      ),
      GoRoute(
        path: '/meals/create',
        name: AppRoute.createMeal.name,
        pageBuilder: (context, state) {
          final extra = state.extra;
          final now = DateTime.now();
          final date = extra is DateTime
              ? extra
              : DateTime(now.year, now.month, now.day);
          return MaterialPage(
            child: CreateMealPage(date: date),
          );
        },
      ),
      GoRoute(
        path: '/meals/edit',
        name: AppRoute.editMeal.name,
        pageBuilder: (context, state) {
          final extra = state.extra;
          if (extra is! Meal) {
            return const MaterialPage(
              child: Scaffold(
                body: Center(
                  child: Text('Meal data not provided'),
                ),
              ),
            );
          }

          return MaterialPage(
            child: EditMealPage(meal: extra),
          );
        },
      ),
      GoRoute(
        path: '/exercises',
        name: 'exercises',
        pageBuilder: (context, state) {
          final extra = state.extra;
          final DateTime? contextDate =
          extra is DateTime ? extra : null;

          return MaterialPage(
            child: ExerciseSearchPage(contextDate: contextDate),
          );
        },
      ),
      GoRoute(
        path: '/exercises/log',
        name: 'exerciseLog',
        pageBuilder: (context, state) {
          final extra = state.extra;
          if (extra is! ExerciseLogPageArgs) {
            return const MaterialPage(
              child: Scaffold(
                body: Center(child: Text('Exercise data not provided')),
              ),
            );
          }

          return MaterialPage(
            child: ExerciseLogPage(args: extra),
          );
        },
      ),
      GoRoute(
        path: '/exercise-logs/edit',
        name: 'editExerciseLog',
        pageBuilder: (context, state) {
          final extra = state.extra;
          if (extra is! EditExerciseLogPageArgs) {
            return const MaterialPage(
              child: Scaffold(
                body: Center(child: Text('Exercise log data not provided')),
              ),
            );
          }

          return MaterialPage(
            child: EditExerciseLogPage(args: extra),
          );
        },
      ),
      GoRoute(
        path: '/meals/items/edit',
        pageBuilder: (context, state) {
          final args = state.extra! as EditMealItemPageArgs;
          return MaterialPage(child: EditMealItemPage(args: args));
        },
      ),
      GoRoute(
        path: '/groceries',
        name: 'groceries',
        pageBuilder: (context, state) {
          final extra = state.extra;
          GrocerySearchPageArgs? args;
          if (extra is GrocerySearchPageArgs) {
            args = extra;
          }
          return MaterialPage(
            child: GrocerySearchPage(args: args),
          );
        },
      ),
      GoRoute(
        path: '/meal-items/create',
        name: 'mealItemCreate',
        pageBuilder: (context, state) {
          final extra = state.extra;
          if (extra is! MealItemCreatePageArgs) {
            return const MaterialPage(
              child: Scaffold(
                body: Center(child: Text('Missing meal item args')),
              ),
            );
          }
          return MaterialPage(
            child: MealItemCreatePage(args: extra),
          );
        },
      ),
      GoRoute(
        path: '/scanner',
        pageBuilder: (_, __) => const MaterialPage(
          child: ScannerPage(),
        ),
      ),
      GoRoute(
        name: AppRoute.userGoal.name,
        path: '/user-goal',
        pageBuilder: (context, state) {
          return const MaterialPage(
            child: UserGoalPage(),
          );
        },
      ),
      GoRoute(
        name: AppRoute.userGoalHistory.name,
        path: '/user-goal-history',
        pageBuilder: (context, state) {
          return const MaterialPage(
            child: UserGoalHistoryPage(),
          );
        },
      ),
      GoRoute(
        path: '/notifications',
        name: AppRoute.notifications.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: NotificationsPage()),
      ),
      GoRoute(
        path: '/achievements',
        name: AppRoute.achievements.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: UserAchievementsPage()),
      ),
      GoRoute(
        path: '/groceries-menu',
        name: AppRoute.groceriesMenu.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: GroceriesMenuPage()),
      ),

      GoRoute(
        path: '/exercises-menu',
        name: AppRoute.exercisesMenu.name,
        pageBuilder: (context, state) =>
        const MaterialPage(child: ExercisesMenuPage()),
      ),
      GoRoute(
          path: '/exercises/suggest',
          name: AppRoute.exercisesSuggest.name,
          pageBuilder: (context, state) =>
          const MaterialPage(child: ExerciseSuggestPage())
      ),
      GoRoute(
          path: '/groceries/suggest',
          name: AppRoute.grocerySuggest.name,
          pageBuilder: (context, state) =>
          const MaterialPage(child: GrocerySuggestPage())
      ),
      GoRoute(
          path: '/two-factor',
          name: AppRoute.twoFactor.name,
          pageBuilder: (context, state) =>
          const MaterialPage(child: TwoFactorPage())
      ),
      GoRoute(
          path: '/two-factor-settings',
          name: AppRoute.twoFactorSettings.name,
          pageBuilder: (context, state) =>
          const MaterialPage(child: TwoFactorSettingsPage())
      ),
      GoRoute(
          path: '/social',
          name: AppRoute.social.name,
          pageBuilder: (context, state) =>
          const MaterialPage(child: SocialPage())
      ),
      GoRoute(
        path: '/groceries/recommended',
        name: 'recommendedGroceries',
        pageBuilder: (context, state) => const MaterialPage(
          child: RecommendedGroceriesPage(),
        ),
      ),
      GoRoute(
        path: '/social/profile/:userId',
        name: 'socialProfile',
        pageBuilder: (context, state) {
          final userId = state.pathParameters['userId'];
          if (userId == null || userId.isEmpty) {
            return const MaterialPage(
              child: Scaffold(
                body: Center(child: Text('Missing userId')),
              ),
            );
          }

          return MaterialPage(
            child: SocialProfilePage(
              args: SocialProfilePageArgs(userId: userId),
            ),
          );
        },
      ),
    ],
  );
}
