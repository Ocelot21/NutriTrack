import 'package:flutter/material.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('NutriTrack'),
      ),
      body: Center(
        child: Text(
          'Login successful.\nThis is temporary home page.',
          style: theme.textTheme.headlineSmall,
          textAlign: TextAlign.center,
        ),
      ),
    );
  }
}
