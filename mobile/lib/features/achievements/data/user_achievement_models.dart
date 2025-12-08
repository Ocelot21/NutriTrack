import 'package:flutter/material.dart';

enum AchievementCategoryUi {
  goals('Goals'),
  exercise('Exercise'),
  meals('Meals'),
  calories('Calories'),
  unknown('Unknown');

  final String label;
  const AchievementCategoryUi(this.label);

  static AchievementCategoryUi fromBackend(dynamic value) {
    if (value is int) {
      switch (value) {
        case 1:
          return AchievementCategoryUi.goals;
        case 2:
          return AchievementCategoryUi.exercise;
        case 3:
          return AchievementCategoryUi.meals;
        case 4:
          return AchievementCategoryUi.calories;
        default:
          return AchievementCategoryUi.unknown;
      }
    }

    if (value is String) {
      switch (value.toLowerCase()) {
        case 'goals':
          return AchievementCategoryUi.goals;
        case 'exercise':
          return AchievementCategoryUi.exercise;
        case 'meals':
          return AchievementCategoryUi.meals;
        case 'calories':
          return AchievementCategoryUi.calories;
        default:
          return AchievementCategoryUi.unknown;
      }
    }

    return AchievementCategoryUi.unknown;
  }
}

class UserAchievementModel {
  final String id;
  final String? achievementId;
  final String key;
  final String title;
  final String description;
  final int points;
  final AchievementCategoryUi category;
  final String? iconName;
  final DateTime earnedAt;

  UserAchievementModel({
    required this.id,
    required this.achievementId,
    required this.key,
    required this.title,
    required this.description,
    required this.points,
    required this.category,
    required this.iconName,
    required this.earnedAt,
  });

  factory UserAchievementModel.fromJson(Map<String, dynamic> json) {
    final rawCategory = json['category'];
    final earnedAtStr =
        json['localDateEarned'] as String ?? json['earnedAtLocal'] as String? ?? json['earnedAtUtc'] as String;

    return UserAchievementModel(
      id: json['id'] as String,
      achievementId: json['achievementId'] as String?,
      key: json['key'] as String,
      title: json['title'] as String,
      description: json['description'] as String,
      points: json['points'] as int,
      category: AchievementCategoryUi.fromBackend(rawCategory),
      iconName: json['iconName'] as String?,
      earnedAt: DateTime.parse(earnedAtStr),
    );
  }
}


IconData achievementIconForName(String? iconName) {
  switch (iconName) {
    case 'flag':
      return Icons.flag;
    case 'flag_circle':
      return Icons.flag_circle;
    case 'military_tech':
      return Icons.military_tech;
    case 'emoji_events':
      return Icons.emoji_events;
    case 'fitness_center':
      return Icons.fitness_center;
    case 'directions_run':
      return Icons.directions_run;
    case 'self_improvement':
      return Icons.self_improvement;
    case 'sports_gymnastics':
      return Icons.sports_gymnastics;
    case 'restaurant':
      return Icons.restaurant;
    case 'restaurant_menu':
      return Icons.restaurant_menu;
    case 'lunch_dining':
      return Icons.lunch_dining;
    case 'fastfood':
      return Icons.fastfood;
    case 'local_fire_department':
      return Icons.local_fire_department;
    case 'whatshot':
      return Icons.whatshot;
    default:
      return Icons.emoji_events_outlined;
  }
}
