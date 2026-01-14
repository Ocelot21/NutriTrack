import 'package:desktop/app/router.dart';
import 'package:flutter/material.dart';

import 'theme/app_theme.dart';

class NutriTrackAdminApp extends StatelessWidget {
  const NutriTrackAdminApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'NutriTrack',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      darkTheme: AppTheme.dark,
      themeMode: ThemeMode.system,
      routerConfig: router
    );
  }
}