import 'package:dio/dio.dart';

import 'api_exception.dart';
import 'problem_details.dart';

class ApiClient {
  final Dio _dio;
  String? _authToken;

  ApiClient(this._dio);

  void setAuthToken(String? token) {
    _authToken = token;

    if (_authToken != null && _authToken!.isNotEmpty) {
      _dio.options.headers['Authorization'] = 'Bearer $_authToken';
    } else {
      _dio.options.headers.remove('Authorization');
    }
  }

  Dio get raw => _dio;

  Future<Response<T>> _wrapRequest<T>(Future<Response<T>> future) async {
    try {
      final response = await future;
      return response;
    } on DioException catch (e) {
      final statusCode = e.response?.statusCode;
      ProblemDetails? problem;
      String message = 'Unexpected network error';

      final data = e.response?.data;

      if (data is Map<String, dynamic>) {
        // Try ProblemDetails first
        try {
          problem = ProblemDetails.fromJson(data);
        } catch (_) {
          problem = null;
        }

        if (problem != null && problem.hasErrors) {
          final msgs = problem.allMessages;
          if (msgs.isNotEmpty) {
            message = msgs.join('\n');
          }
        } else if (problem != null && problem.title != null) {
          message = problem.title!;
        } else {
          // Not ProblemDetails - try common fields
          final dynamic title = data['title'];
          final dynamic msg = data['message'] ?? data['error'] ?? data['detail'];
          if (msg is String && msg.trim().isNotEmpty) {
            message = msg.trim();
          } else if (title is String && title.trim().isNotEmpty) {
            message = title.trim();
          } else if (statusCode != null) {
            message = 'Request failed ($statusCode)';
          }
        }
      } else if (data is String && data.isNotEmpty) {
        message = data;
      } else if (e.message != null && e.message!.isNotEmpty) {
        message = e.message!;
      } else if (statusCode != null) {
        message = 'Request failed ($statusCode)';
      }

      if (statusCode == 401) {
        message = 'You are not authorized. Please log in again.';
      }

      throw ApiException(
        message: message,
        statusCode: statusCode,
        problem: problem,
      );
    } catch (e) {
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

  Future<T> postMultipart<T>(
      String path,
      FormData formData,
      ) async {
    final response = await _dio.post<T>(
      path,
      data: formData,
      options: Options(
        contentType: 'multipart/form-data',
      ),
    );

    return response.data as T;
  }

  Future<T> patch<T>(
      String path, {
        dynamic data,
        Map<String, dynamic>? queryParameters,
        Options? options,
        T Function(dynamic json)? decoder,
      }) async {
    final response = await _dio.patch(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );

    if (decoder != null) {
      return decoder(response.data);
    }

    return response.data as T;
  }
}
