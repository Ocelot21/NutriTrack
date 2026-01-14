class ProblemDetails {
  final String? title;
  final String? type;
  final String? detail;
  final int? status;
  final String? traceId;

  /// Validation / domain errors returned by the backend.
  /// Keys are usually property names or error codes, e.g.:
  /// - "FirstName"
  /// - "Authentication.InvalidCredentials"
  final Map<String, List<String>> errors;

  const ProblemDetails({
    this.title,
    this.type,
    this.detail,
    this.status,
    this.traceId,
    this.errors = const {},
  });

  factory ProblemDetails.fromJson(Map<String, dynamic> json) {
    Map<String, List<String>> parsedErrors = const {};

    final rawErrors = json['errors'];
    if (rawErrors is Map) {
      parsedErrors = rawErrors.map((key, value) {
        final list = <String>[];

        if (value is List) {
          for (final v in value) {
            if (v == null) continue;
            list.add(v.toString());
          }
        } else if (value != null) {
          // Defensive: single string or something else
          list.add(value.toString());
        }

        return MapEntry(key.toString(), list);
      });
    }

    int? status;
    final rawStatus = json['status'];
    if (rawStatus is int) {
      status = rawStatus;
    } else if (rawStatus is num) {
      status = rawStatus.toInt();
    }

    return ProblemDetails(
      title: json['title'] as String?,
      type: json['type'] as String?,
      detail: json['detail'] as String?,
      status: status,
      traceId: json['traceId'] as String?,
      errors: parsedErrors,
    );
  }

  bool get hasErrors => errors.isNotEmpty;

  /// All keys from the `errors` dictionary.
  List<String> get errorKeys => errors.keys.toList(growable: false);

  /// Flattens all error messages into a single list.
  List<String> get allMessages =>
      errors.values.expand((list) => list).toList(growable: false);

  /// Returns the first error message for a given key, or null.
  String? firstForKey(String key) {
    final list = errors[key];
    if (list == null || list.isEmpty) return null;
    return list.first;
  }

  /// Tries to find the first error where the key starts with [prefix].
  /// Useful for things like "Authentication." or "User." error codes.
  String? firstForKeyPrefix(String prefix) {
    for (final entry in errors.entries) {
      if (entry.key.startsWith(prefix) && entry.value.isNotEmpty) {
        return entry.value.first;
      }
    }
    return null;
  }

  /// Returns a "best effort" single message for display:
  /// - first validation message if available
  /// - otherwise `detail`
  /// - otherwise `title`
  String? get primaryMessage {
    if (hasErrors) {
      final first = allMessages.firstOrNull;
      if (first != null && first.isNotEmpty) return first;
    }
    if (detail != null && detail!.isNotEmpty) return detail;
    if (title != null && title!.isNotEmpty) return title;
    return null;
  }
}

// Small extension just so we don't import collection libs everywhere.
extension _FirstOrNullExtension<T> on Iterable<T> {
  T? get firstOrNull => isEmpty ? null : first;
}
