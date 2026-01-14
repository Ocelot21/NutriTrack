import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class GroceriesMenuPage extends StatelessWidget {
  const GroceriesMenuPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Groceries'),
      ),
      body: SafeArea(
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _MenuCard(
                  icon: Icons.list_alt,
                  title: 'Browse groceries',
                  subtitle:
                  'Search and filter existing foods in the database.',
                  onTap: () {
                    context.push('/groceries');
                  },
                ),
                const SizedBox(height: 12),
                _MenuCard(
                  icon: Icons.auto_awesome,
                  title: 'Recommended for you',
                  subtitle: 'Personalized suggestions powered by AI.',
                  onTap: () {
                    context.push('/groceries/recommended');
                  },
                ),
                const SizedBox(height: 12),
                _MenuCard(
                  icon: Icons.add_circle_outline,
                  title: 'Suggest a grocery',
                  subtitle:
                  'Propose a new food. Our admins will review it before it appears in NutriTrack.',
                    onTap: () {
                      context.push('/groceries/suggest');
                    },
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _MenuCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  const _MenuCard({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: theme.colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          children: [
            Icon(icon, size: 32, color: theme.colorScheme.primary),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: theme.textTheme.titleMedium
                        ?.copyWith(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: theme.textTheme.bodyMedium,
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}