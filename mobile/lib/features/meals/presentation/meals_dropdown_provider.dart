import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/meals_repo.dart';
import '../data/meal_dropdown_models.dart';
import 'meal_item_providers.dart';

final mealsForDropdownProvider =
FutureProvider.family<List<MealDropdownItem>, DateTime?>((ref, centerDate) {
  final repo = ref.read(mealsRepoProvider);
  return repo.listMealsAroundDate(centerDate: centerDate);
});
