import '../../achievements/data/user_achievement_models.dart';

enum SocialPostTypeUi {
  text(1),
  achievementShare(2),
  dailyOverviewShare(3),
  goalProgressShare(4),
  unknown(-1);

  final int value;
  const SocialPostTypeUi(this.value);

  static SocialPostTypeUi fromInt(dynamic v) {
    if (v is int) {
      switch (v) {
        case 1:
          return SocialPostTypeUi.text;
        case 2:
          return SocialPostTypeUi.achievementShare;
        case 3:
          return SocialPostTypeUi.dailyOverviewShare;
        case 4:
          return SocialPostTypeUi.goalProgressShare;
        default:
          return SocialPostTypeUi.unknown;
      }
    }
    return SocialPostTypeUi.unknown;
  }
}

enum SocialPostVisibilityUi {
  public(1),
  unknown(-1);

  final int value;
  const SocialPostVisibilityUi(this.value);

  static SocialPostVisibilityUi fromInt(dynamic v) {
    if (v is int) {
      switch (v) {
        case 1:
          return SocialPostVisibilityUi.public;
        default:
          return SocialPostVisibilityUi.unknown;
      }
    }
    return SocialPostVisibilityUi.unknown;
  }
}

class SocialPostAuthorModel {
  final String id;
  final String username;
  final String? avatarUrl;

  SocialPostAuthorModel({
    required this.id,
    required this.username,
    required this.avatarUrl,
  });

  factory SocialPostAuthorModel.fromJson(Map<String, dynamic> json) {
    return SocialPostAuthorModel(
      id: json['id'] as String,
      username: json['username'] as String,
      avatarUrl: json['avatarUrl'] as String?,
    );
  }
}

class DailyOverviewSnapshotModel {
  final String id;
  final DateTime date; // DateOnly -> "yyyy-MM-dd"
  final int targetCalories;
  final double targetProteinGrams;
  final double targetFatGrams;
  final double targetCarbohydrateGrams;
  final int consumedCalories;
  final int burnedCalories;
  final int netCalories;
  final int remainingCalories;
  final double consumedProteinGrams;
  final double consumedFatGrams;
  final double consumedCarbohydrateGrams;
  final int mealCount;
  final int exerciseCount;

  DailyOverviewSnapshotModel({
    required this.id,
    required this.date,
    required this.targetCalories,
    required this.targetProteinGrams,
    required this.targetFatGrams,
    required this.targetCarbohydrateGrams,
    required this.consumedCalories,
    required this.burnedCalories,
    required this.netCalories,
    required this.remainingCalories,
    required this.consumedProteinGrams,
    required this.consumedFatGrams,
    required this.consumedCarbohydrateGrams,
    required this.mealCount,
    required this.exerciseCount,
  });

  factory DailyOverviewSnapshotModel.fromJson(Map<String, dynamic> json) {
    String dateStr = (json['date'] ?? json['day'] ?? json['snapshotDate']) as String;

    double toDouble(dynamic v) {
      if (v is int) return v.toDouble();
      if (v is double) return v;
      if (v is String) return double.tryParse(v) ?? 0.0;
      return 0.0;
    }

    int toInt(dynamic v) {
      if (v is int) return v;
      if (v is double) return v.toInt();
      if (v is String) return int.tryParse(v) ?? 0;
      return 0;
    }

    return DailyOverviewSnapshotModel(
      id: json['id'] as String,
      date: DateTime.parse(dateStr),
      targetCalories: toInt(json['targetCalories']),
      targetProteinGrams: toDouble(json['targetProteinGrams']),
      targetFatGrams: toDouble(json['targetFatGrams']),
      targetCarbohydrateGrams: toDouble(json['targetCarbohydrateGrams']),
      consumedCalories: toInt(json['consumedCalories']),
      burnedCalories: toInt(json['burnedCalories']),
      netCalories: toInt(json['netCalories']),
      remainingCalories: toInt(json['remainingCalories']),
      consumedProteinGrams: toDouble(json['consumedProteinGrams']),
      consumedFatGrams: toDouble(json['consumedFatGrams']),
      consumedCarbohydrateGrams: toDouble(json['consumedCarbohydrateGrams']),
      mealCount: toInt(json['mealCount']),
      exerciseCount: toInt(json['exerciseCount']),
    );
  }
}

class GoalProgressSnapshotPointModel {
  final DateTime date; // DateOnly
  final double weightKg;

  GoalProgressSnapshotPointModel({
    required this.date,
    required this.weightKg,
  });

  factory GoalProgressSnapshotPointModel.fromJson(Map<String, dynamic> json) {
    final dateStr = json['date'] as String;
    double toDouble(dynamic v) {
      if (v is int) return v.toDouble();
      if (v is double) return v;
      if (v is String) return double.tryParse(v) ?? 0.0;
      return 0.0;
    }

    return GoalProgressSnapshotPointModel(
      date: DateTime.parse(dateStr),
      weightKg: toDouble(json['weightKg']),
    );
  }
}

class GoalProgressSnapshotModel {
  final String id;
  final String userGoalId;
  final String goalType;
  final DateTime goalStartDate;
  final DateTime goalTargetDate;
  final DateTime snapshotDate;
  final double startWeightKg;
  final double currentWeightKg;
  final double targetWeightKg;
  final List<GoalProgressSnapshotPointModel> points;

  GoalProgressSnapshotModel({
    required this.id,
    required this.userGoalId,
    required this.goalType,
    required this.goalStartDate,
    required this.goalTargetDate,
    required this.snapshotDate,
    required this.startWeightKg,
    required this.currentWeightKg,
    required this.targetWeightKg,
    required this.points,
  });

  factory GoalProgressSnapshotModel.fromJson(Map<String, dynamic> json) {
    String goalStart = json['goalStartDate'] as String;
    String goalTarget = json['goalTargetDate'] as String;
    String snapshot = json['snapshotDate'] as String;

    double toDouble(dynamic v) {
      if (v is int) return v.toDouble();
      if (v is double) return v;
      if (v is String) return double.tryParse(v) ?? 0.0;
      return 0.0;
    }

    final pointsJson = (json['points'] as List<dynamic>?) ?? <dynamic>[];

    return GoalProgressSnapshotModel(
      id: json['id'] as String,
      userGoalId: json['userGoalId'] as String,
      goalType: json['goalType'] as String,
      goalStartDate: DateTime.parse(goalStart),
      goalTargetDate: DateTime.parse(goalTarget),
      snapshotDate: DateTime.parse(snapshot),
      startWeightKg: toDouble(json['startWeightKg']),
      currentWeightKg: toDouble(json['currentWeightKg']),
      targetWeightKg: toDouble(json['targetWeightKg']),
      points: pointsJson
          .map((e) => GoalProgressSnapshotPointModel.fromJson(
              Map<String, dynamic>.from(e as Map)))
          .toList(),
    );
  }
}

class SocialPostModel {
  final String id;
  final SocialPostAuthorModel author;
  final SocialPostTypeUi type;
  final SocialPostVisibilityUi visibility;
  final DateTime localTime;
  final String? text;

  /// Present only for achievement posts
  final UserAchievementModel? userAchievement;

  /// Present only for daily overview snapshot posts
  final DailyOverviewSnapshotModel? dailyOverviewSnapshot;

  /// Present only for goal progress snapshot posts
  final GoalProgressSnapshotModel? goalProgressSnapshot;

  SocialPostModel({
    required this.id,
    required this.author,
    required this.type,
    required this.visibility,
    required this.localTime,
    required this.text,
    required this.userAchievement,
    required this.dailyOverviewSnapshot,
    required this.goalProgressSnapshot,
  });

  factory SocialPostModel.fromJson(Map<String, dynamic> json) {
    final uaJson = json['userAchievement'] ?? json['userAchievementResponse'];

    // daily overview might be 'dailyOverview' or 'dailyOverviewSnapshot'
    final dailyOverviewJson = json['dailyOverview'] ?? json['dailyOverviewSnapshot'];

    // goal progress might be 'goalProgress' or 'goalProgressSnapshot'
    final goalProgressJson = json['goalProgress'] ?? json['goalProgressSnapshot'];

    DateTime parseDateTime(dynamic v) {
      if (v is String) return DateTime.parse(v);
      if (v is DateTime) return v;
      return DateTime.now();
    }

    return SocialPostModel(
      id: json['id'] as String,
      author: SocialPostAuthorModel.fromJson(
        Map<String, dynamic>.from(json['author'] as Map),
      ),
      type: SocialPostTypeUi.fromInt(json['type']),
      visibility: SocialPostVisibilityUi.fromInt(json['visibility']),
      localTime: parseDateTime(json['localTime']),
      text: json['text'] as String?,
      userAchievement: uaJson == null
          ? null
          : UserAchievementModel.fromJson(
              Map<String, dynamic>.from(uaJson as Map),
            ),
      dailyOverviewSnapshot: dailyOverviewJson == null
          ? null
          : DailyOverviewSnapshotModel.fromJson(
              Map<String, dynamic>.from(dailyOverviewJson as Map),
            ),
      goalProgressSnapshot: goalProgressJson == null
          ? null
          : GoalProgressSnapshotModel.fromJson(
              Map<String, dynamic>.from(goalProgressJson as Map),
            ),
    );
  }
}

class SocialProfileAchievementModel {
  final String name;
  final String icon;

  SocialProfileAchievementModel({
    required this.name,
    required this.icon,
  });

  factory SocialProfileAchievementModel.fromJson(Map<String, dynamic> json) {
    return SocialProfileAchievementModel(
      name: json['name'] as String,
      icon: json['icon'] as String,
    );
  }
}

class SocialProfileModel {
  final String userId;
  final String username;
  final String? avatarUrl;
  final int totalPoints;
  final List<SocialPostModel> posts;
  final List<SocialProfileAchievementModel> achievements;

  SocialProfileModel({
    required this.userId,
    required this.username,
    required this.avatarUrl,
    required this.totalPoints,
    required this.posts,
    required this.achievements,
  });

  factory SocialProfileModel.fromJson(Map<String, dynamic> json) {
    final postsJson = (json['posts'] as List<dynamic>?) ?? const [];
    final achievementsJson = (json['achievements'] as List<dynamic>?) ?? const [];

    return SocialProfileModel(
      userId: (json['userId'] as String),
      username: json['username'] as String,
      avatarUrl: json['avatarUrl'] as String?,
      totalPoints: (json['totalPoints'] as num?)?.toInt() ?? 0,
      posts: postsJson
          .map((e) => SocialPostModel.fromJson(Map<String, dynamic>.from(e as Map)))
          .toList(),
      achievements: achievementsJson
          .map((e) => SocialProfileAchievementModel.fromJson(
                Map<String, dynamic>.from(e as Map),
              ))
          .toList(),
    );
  }
}
