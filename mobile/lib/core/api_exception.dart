import 'problem_details.dart';

class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final ProblemDetails? problem;

  ApiException({
    required this.message,
    this.statusCode,
    this.problem,
  });

  bool get hasProblemDetails => problem != null;

  Map<String, List<String>> get validationErrors =>
      problem?.errors ?? const {};

  List<String> get allMessages {
    if (problem != null && problem!.hasErrors) {
      return problem!.allMessages;
    }
    return [message];
  }

  @override
  String toString() =>
      'ApiException(statusCode: $statusCode, message: $message)';
}
