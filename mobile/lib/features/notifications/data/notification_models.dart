import '../../../core/models/paged_response.dart';

String? _readOptionalString(Map<String, dynamic> json, String key) {
  final value = json[key];
  if (value is String && value.trim().isNotEmpty) {
    return value;
  }
  return null;
}

DateTime _parseOrNow(String? value) {
  if (value == null) return DateTime.now();
  try {
    return DateTime.parse(value);
  } catch (_) {
    return DateTime.now();
  }
}

DateTime? _parseOrNull(String? value) {
  if (value == null || value.trim().isEmpty) return null;
  try {
    return DateTime.parse(value);
  } catch (_) {
    return null;
  }
}


enum NotificationTypeUi {
  general('General'),
  system('System'),
  reminder('Reminder'),
  achievementUnlocked('Achievement unlocked');

  final String label;
  const NotificationTypeUi(this.label);

  static NotificationTypeUi fromBackend(String value) {
    switch (value.toLowerCase()) {
      case 'achievementunlocked':
      case 'achievement_unlocked':
        return NotificationTypeUi.achievementUnlocked;
      case 'system':
        return NotificationTypeUi.system;
      case 'reminder':
        return NotificationTypeUi.reminder;
      default:
        return NotificationTypeUi.general;
    }
  }
}

enum NotificationStatusUi {
  unread,
  read,
  archived;

  static NotificationStatusUi fromBackend(String value) {
    switch (value.toLowerCase()) {
      case 'read':
        return NotificationStatusUi.read;
      case 'archived':
        return NotificationStatusUi.archived;
      default:
        return NotificationStatusUi.unread;
    }
  }
}

class AppNotification {
  final String id;
  final String title;
  final String message;
  final NotificationTypeUi type;
  final NotificationStatusUi status;
  final DateTime occurredAt;
  final DateTime? readAt;
  final String? linkUrl;
  final String? metadataJson;

  bool get isUnread => status == NotificationStatusUi.unread;

  AppNotification({
    required this.id,
    required this.title,
    required this.message,
    required this.type,
    required this.status,
    required this.occurredAt,
    this.readAt,
    this.linkUrl,
    this.metadataJson,
  });

  factory AppNotification.fromJson(Map<String, dynamic> json) {
    final typeStr = json['type'] as String? ?? 'General';
    final statusStr = json['status'] as String? ?? 'Unread';

    final occurredStr =
        _readOptionalString(json, 'occurredAtLocal') ??
            _readOptionalString(json, 'occurredAtUtc') ??
            _readOptionalString(json, 'occurredAtUtc');

    final readStr =
        _readOptionalString(json, 'readAtLocal') ??
            _readOptionalString(json, 'readAtUtc');

    return AppNotification(
      id: json['id'] as String,
      title: json['title'] as String,
      message: json['message'] as String,
      type: NotificationTypeUi.fromBackend(typeStr),
      status: NotificationStatusUi.fromBackend(statusStr),
      occurredAt: _parseOrNow(occurredStr),
      readAt: _parseOrNull(readStr),
      linkUrl: json['linkUrl'] as String?,
      metadataJson: json['metadataJson'] as String?,
    );
  }
}
