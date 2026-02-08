class AppConfig {
  static const String apiBaseUrl =
  String.fromEnvironment('API_BASE_URL', defaultValue: '');

  static String get resolvedApiBaseUrl {
    if (apiBaseUrl.isNotEmpty) return apiBaseUrl;

    return 'http://10.0.2.2:5000/api';
  }
}