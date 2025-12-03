import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'api_client.dart';
import 'token_store.dart';


/// - Emulator →  `http://10.0.2.2:5071/api`
/// - Smartphone → IP of PC `http://192.168.0.29:5071/api`
final baseUrlProvider = Provider<String>((ref) {
  return 'http://192.168.0.29:5071/api';
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
