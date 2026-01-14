import 'package:dio/dio.dart';
import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import '../lib/features/dashboard/data/dashboard_repo.dart';

Future<void> main() async {
  final baseUrl = 'http://192.168.0.29:5071/api';
  final dio = Dio(BaseOptions(baseUrl: baseUrl));
  final api = ApiClient(dio);

  final store = const TokenStore();
  final token = await store.read();
  print('Token present: ${token != null && token.isNotEmpty}');

  api.setAuthToken(token);

  final repo = DashboardRepo(api, store);
  try {
    final resp = await repo.getSummary(days: 7, top: 10);
    print('OK: activeUsers=${resp.kpis.activeUsers} meals=${resp.kpis.mealsLogged} exercises=${resp.kpis.exerciseLogsLogged}');
  } catch (e) {
    print('ERROR: $e');
  }
}

