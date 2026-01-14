import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/token_store.dart';
import 'package:nutritrack_mobile/core/config/app_config.dart';


/// - Emulator →  `http://10.0.2.2:5000/api`
/// - Smartphone → IP of PC `http://192.168.0.29:5000/api`
final baseUrlProvider = Provider<String>((ref) {
  return AppConfig.resolvedApiBaseUrl;
});

final dioProvider = Provider<Dio>((ref) {
  final baseUrl = ref.watch(baseUrlProvider);

  return Dio(
    BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ),
  );
});

final apiClientProvider = Provider<ApiClient>((ref) {
  final dio = ref.watch(dioProvider);
  return ApiClient(dio);
});

final tokenStoreProvider = Provider<TokenStore>((ref) {
  return TokenStore();
});
