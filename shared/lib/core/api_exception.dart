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

  /// Validation errors returned by the backend (if any).
  Map<String, List<String>> get validationErrors =>
      problem?.errors ?? const {};

  /// All validation messages if present, otherwise the generic message.
  List<String> get allMessages {
    if (problem != null && problem!.hasErrors) {
      return problem!.allMessages;
    }
    return [message];
  }

  /// A single "primary" message suitable for showing in a Snackbar/Dialog.
  String get primaryMessage {
    final fromProblem = problem?.primaryMessage;
    if (fromProblem != null && fromProblem.isNotEmpty) {
      return fromProblem;
    }
    return message;
  }

  /// Convenience helpers for common HTTP statuses.
  bool get isUnauthorized => statusCode == 401;
  bool get isForbidden => statusCode == 403;
  bool get isNotFound => statusCode == 404;
  bool get isValidationError => statusCode == 400 && problem?.hasErrors == true;
  bool get isServerError =>
      statusCode != null && statusCode! >= 500 && statusCode! < 600;

  /// Helpful for debugging / logging.
  @override
  String toString() {
    final sc = statusCode != null ? ' ($statusCode)' : '';
    return 'ApiException$sc: $message';
  }
}
