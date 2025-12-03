class ProblemDetails {
  final String? title;
  final String? type;
  final int? status;
  final Map<String, List<String>>? errors;

  ProblemDetails({
    this.title,
    this.type,
    this.status,
    this.errors,
  });

  factory ProblemDetails.fromJson(Map<String, dynamic> json) {
    Map<String, List<String>>? parsedErrors;

    if (json['errors'] != null && json['errors'] is Map) {
      parsedErrors = (json['errors'] as Map).map(
            (key, value) => MapEntry(
          key.toString(),
          List<String>.from(value as List),
        ),
      );
    }

    return ProblemDetails(
      title: json['title'] as String?,
      type: json['type'] as String?,
      status: json['status'] as int?,
      errors: parsedErrors,
    );
  }

  bool get hasErrors => errors != null && errors!.isNotEmpty;

  List<String> get errorKeys => errors?.keys.toList() ?? const [];

  List<String> get allMessages {
    if (!hasErrors) return [];
    return errors!.values.expand((list) => list).toList();
  }
}
