import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/auth_providers.dart';
import '../../features/notifications/presentation/notification_providers.dart';


class AppDrawer extends ConsumerWidget {
  const AppDrawer({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final unreadCountAsync = ref.watch(unreadNotificationCountProvider);

    return Drawer(
      child: SafeArea(
        child: Column(
          children: [
            const DrawerHeader(
              child: Text(
                'NutriTrack',
                style: TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                ),
              ),
              ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Home'),
              onTap: () {
                Navigator.of(context).pop();
                context.go('/home');
              },
            ),
            ListTile(
              leading: const Icon(Icons.person),
              title: const Text('Profile'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/profile');
              },
            ),
            ListTile(
              leading: const Icon(Icons.fastfood),
              title: const Text('Groceries'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/groceries-menu');
              },
            ),
            ListTile(
              leading: const Icon(Icons.fitness_center),
              title: const Text('Exercises'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/exercises-menu');
              },
            ),
            ListTile(
              leading: const Icon(Icons.flag_outlined),
              title: const Text('Goals'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/user-goal');
              },
            ),
            ListTile(
              leading: Badge(
                isLabelVisible: unreadCountAsync.when(
                  data: (count) => count > 0,
                  loading: () => false,
                  error: (_, __) => false,
                ),
                label: unreadCountAsync.when(
                  data: (count) => Text(count > 99 ? '99+' : count.toString()),
                  loading: () => const Text(''),
                  error: (_, __) => const Text(''),
                ),
                child: const Icon(Icons.notifications),
              ),
              title: const Text('Notifications'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/notifications');
              },
            ),
            ListTile(
              leading: const Icon(Icons.emoji_events_outlined),
              title: const Text('Achievements'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/achievements');
              },
            ),
            ListTile(
              leading: const Icon(Icons.dynamic_feed_rounded),
              title: const Text('Social'),
              onTap: () {
                Navigator.of(context).pop();
                context.push('/social');
              },
            ),

            const Spacer(),
            ListTile(
              leading: const Icon(Icons.logout, color: Colors.red),
              title: const Text('Logout'),
              onTap: () async {
                final ok = await _confirmLogout(context);
                if (ok != true) return;

                await ref.read(authRepoProvider).logout();

                if (context.mounted) Navigator.of(context).pop();
                if (context.mounted) context.go('/login');
                if (context.mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text('Logged out')),
                  );
                }
              },
            ),
          ],
        ),
      ),
    );
  }
}

Future<bool?> _confirmLogout(BuildContext context) {
  return showDialog<bool>(
    context: context,
    builder: (ctx) => AlertDialog(
      title: const Text('Log out'),
      content: const Text('Are you sure you want to log out?'),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(ctx, false),
          child: const Text('Cancel'),
        ),
        FilledButton(
          onPressed: () => Navigator.pop(ctx, true),
          child: const Text('Log out'),
        ),
      ],
    ),
  );
}
