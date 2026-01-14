import 'package:dio/dio.dart';

import 'api_exception.dart';
import 'problem_details.dart';

/// Thin HTTP client wrapper around Dio that:
/// - attaches/removes Authorization header based on the current token
/// - normalises error handling into [ApiException]
/// - parses RFC7807 ProblemDetails responses when available
class ApiClient {
  final Dio _dio;
  String? _authToken;

  ApiClient(this._dio);

  /// Sets or clears the bearer token used for all subsequent requests.
  void setAuthToken(String? token) {
    _authToken = token;

    if (_authToken != null && _authToken!.isNotEmpty) {
      _dio.options.headers['Authorization'] = 'Bearer $_authToken';
    } else {
      _dio.options.headers.remove('Authorization');
    }
  }

  /// Gives access to the underlying Dio instance if needed.
  Dio get raw => _dio;

  /// Wraps a Dio request and converts [DioException] into [ApiException],
  /// trying to parse ProblemDetails if the response body matches that shape.
  Future<Response<T>> _wrapRequest<T>(Future<Response<T>> future) async {
    try {
      final response = await future;
      return response;
    } on DioException catch (e) {
      final statusCode = e.response?.statusCode;
      ProblemDetails? problem;
      String message = 'Unexpected network error. Please try again.';

      final data = e.response?.data;

      // Try to parse ProblemDetails if the response body looks like a JSON object.
      if (data is Map<String, dynamic>) {
        problem = ProblemDetails.fromJson(data);
      } else if (data is Map) {
        problem = ProblemDetails.fromJson(
          Map<String, dynamic>.from(data),
        );
      }

      // Prefer messages from ProblemDetails, if available.
      final fromProblem = problem?.primaryMessage;
      if (fromProblem != null && fromProblem.isNotEmpty) {
        message = fromProblem;
      } else if (data is String && data.isNotEmpty) {
        // Fallback: plain string body
        message = data;
      } else if (e.message != null && e.message!.isNotEmpty) {
        // Fallback: Dio's own message
        message = e.message!;
      }

      // Customise some common statuses if backend didn't provide anything nicer.
      if (statusCode == 401 && (fromProblem == null || fromProblem.isEmpty)) {
        message = 'You are not authorized. Please log in again.';
      }

      throw ApiException(
        message: message,
        statusCode: statusCode,
        problem: problem,
      );
    } catch (e) {
      // Any non-Dio errors end up here.
      throw ApiException(message: 'Unexpected error: $e');
    }
  }

  Future<Response<T>> post<T>(
      String path, {
        dynamic data,
        Map<String, dynamic>? queryParameters,
      }) {
    return _wrapRequest(
      _dio.post<T>(
        path,
        data: data,
        queryParameters: queryParameters,
      ),
    );
  }

  Future<Response<T>> get<T>(
      String path, {
        Map<String, dynamic>? queryParameters,
      }) {
    return _wrapRequest(
      _dio.get<T>(
        path,
        queryParameters: queryParameters,
      ),
    );
  }

  Future<Response<T>> put<T>(
      String path, {
        dynamic data,
        Map<String, dynamic>? queryParameters,
      }) {
    return _wrapRequest(
      _dio.put<T>(
        path,
        data: data,
        queryParameters: queryParameters,
      ),
    );
  }

  Future<Response<T>> delete<T>(
      String path, {
        Map<String, dynamic>? queryParameters,
      }) {
    return _wrapRequest(
      _dio.delete<T>(
        path,
        queryParameters: queryParameters,
      ),
    );
  }

  /// Sends a multipart/form-data POST request, wrapping errors into [ApiException].
  Future<T> postMultipart<T>(
      String path,
      FormData formData,
      ) async {
    final response = await _wrapRequest<T>(
      _dio.post<T>(
        path,
        data: formData,
        options: Options(
          contentType: 'multipart/form-data',
        ),
      ),
    );

    return response.data as T;
  }

  /// Sends a multipart/form-data PUT request, wrapping errors into [ApiException].
  Future<T> putMultipart<T>(
      String path,
      FormData formData,
      ) async {
    final response = await _wrapRequest<T>(
      _dio.put<T>(
        path,
        data: formData,
        options: Options(
          contentType: 'multipart/form-data',
        ),
      ),
    );

    return response.data as T;
  }

  /// PATCH helper with optional custom decoder.
  ///
  /// The request still goes through [_wrapRequest] so all errors become [ApiException].
  Future<T> patch<T>(
      String path, {
        dynamic data,
        Map<String, dynamic>? queryParameters,
        Options? options,
        T Function(dynamic json)? decoder,
      }) async {
    final response = await _wrapRequest<dynamic>(
      _dio.patch<dynamic>(
        path,
        data: data,
        queryParameters: queryParameters,
        options: options,
      ),
    );

    if (decoder != null) {
      return decoder(response.data);
    }

    return response.data as T;
  }
}
