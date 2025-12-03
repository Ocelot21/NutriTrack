import 'package:dio/dio.dart';

import 'api_exception.dart';
import 'problem_details.dart';

class ApiClient {
  final Dio _dio;

  ApiClient(this._dio);

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
        problem = ProblemDetails.fromJson(data);

        if (problem.hasErrors) {
          final messages = problem.allMessages;
          if (messages.isNotEmpty) {
            message = messages.join('\n');
          }
        } else if (problem.title != null) {
          message = problem.title!;
        }
      } else if (data is String && data.isNotEmpty) {
        message = data;
      } else if (e.message != null && e.message!.isNotEmpty) {
        message = e.message!;
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

// Put and delete methods can be added similarly
}
