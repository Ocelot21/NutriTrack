class Me {
  final String id;
  final String username;
  final String email;
  final String firstName;
  final String lastName;
  final String? role;
  final bool isEmailVerified;
  final String? avatarUrl;
  final String timeZoneId;
  final DateTime? lastLoginAtUtc;
  final String? country;
  final bool isHealthProfileCompleted;
  final String gender;
  final String activityLevel;
  final DateTime? birthdate;
  final double? heightCm;
  final double? weightKg;

  Me({
    required this.id,
    required this.username,
    required this.email,
    required this.firstName,
    required this.lastName,
    required this.role,
    required this.isEmailVerified,
    required this.avatarUrl,
    required this.timeZoneId,
    required this.lastLoginAtUtc,
    required this.country,
    required this.isHealthProfileCompleted,
    required this.gender,
    required this.activityLevel,
    required this.birthdate,
    required this.heightCm,
    required this.weightKg,
  });

  factory Me.fromJson(Map<String, dynamic> json) {
    DateTime? parseNullableDateTime(dynamic v) {
      if (v == null) return null;
      return DateTime.parse(v as String);
    }

    DateTime? parseNullableDateOnly(dynamic v) {
      if (v == null) return null;
      return DateTime.parse(v as String);
    }

    double? parseNullableDecimal(dynamic v) {
      if (v == null) return null;
      return (v as num).toDouble();
    }

    return Me(
      id: json['id'] as String,
      username: json['username'] as String,
      email: json['email'] as String,
      firstName: json['firstName'] as String,
      lastName: json['lastName'] as String,
      role: json['role'] as String?,
      isEmailVerified: json['isEmailVerified'] as bool? ?? false,
      avatarUrl: json['avatarUrl'] as String?,
      timeZoneId: json['timeZoneId'] as String,
      lastLoginAtUtc: parseNullableDateTime(json['lastLoginAtUtc']),
      country: json['country'] as String?,
      isHealthProfileCompleted:
      json['isHealthProfileCompleted'] as bool? ?? false,
      gender: json['gender'] as String,
      activityLevel: json['activityLevel'] as String,
      birthdate: parseNullableDateOnly(json['birthdate']),
      heightCm: parseNullableDecimal(json['heightCm']),
      weightKg: parseNullableDecimal(json['weightKg']),
    );
  }
}
