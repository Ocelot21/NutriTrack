class Me {
  final String id;
  final String username;
  final String email;
  final String firstName;
  final String lastName;
  final String? role;
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
      role: json['roleName'] as String?,
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

/// Mirrors backend `UserResponse`.
class UserResponse {
  final String id;
  final String username;
  final String email;
  final String firstName;
  final String lastName;
  final String? roleId;
  final String? roleName;
  final String? avatarUrl;
  final String timeZoneId;
  final DateTime? lastLoginAtUtc;
  final String? countryCode;
  final bool isHealthProfileCompleted;
  final String gender;
  final String activityLevel;
  final DateTime? birthdate; // DateOnly in backend
  final double? heightCm;
  final double? weightKg;
  final bool isTwoFactorEnabled;

  UserResponse({
    required this.id,
    required this.username,
    required this.email,
    required this.firstName,
    required this.lastName,
    required this.roleId,
    required this.roleName,
    required this.avatarUrl,
    required this.timeZoneId,
    required this.lastLoginAtUtc,
    required this.countryCode,
    required this.isHealthProfileCompleted,
    required this.gender,
    required this.activityLevel,
    required this.birthdate,
    required this.heightCm,
    required this.weightKg,
    required this.isTwoFactorEnabled,
  });

  factory UserResponse.fromJson(Map<String, dynamic> json) {
    DateTime? parseNullableDateTime(dynamic v) {
      if (v == null) return null;
      return DateTime.parse(v as String);
    }

    double? parseNullableDecimal(dynamic v) {
      if (v == null) return null;
      return (v as num).toDouble();
    }

    return UserResponse(
      id: json['id'] as String,
      username: json['username'] as String,
      email: json['email'] as String,
      firstName: json['firstName'] as String,
      lastName: json['lastName'] as String,
      roleId: json['roleId'] as String?,
      roleName: json['roleName'] as String?,
      avatarUrl: json['avatarUrl'] as String?,
      timeZoneId: json['timeZoneId'] as String,
      lastLoginAtUtc: parseNullableDateTime(json['lastLoginAtUtc']),
      countryCode: json['countryCode'] as String?,
      isHealthProfileCompleted: json['isHealthProfileCompleted'] as bool? ?? false,
      gender: json['gender'] as String,
      activityLevel: json['activityLevel'] as String,
      birthdate: parseNullableDateTime(json['birthdate']),
      heightCm: parseNullableDecimal(json['heightCm']),
      weightKg: parseNullableDecimal(json['weightKg']),
      isTwoFactorEnabled: json['isTwoFactorEnabled'] as bool? ?? false,
    );
  }
}

class SetUserRoleRequest {
  final String roleId;
  const SetUserRoleRequest(this.roleId);

  Map<String, dynamic> toJson() => {
        'roleId': roleId,
      };
}
