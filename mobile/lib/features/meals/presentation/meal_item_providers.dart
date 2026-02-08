import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/meals_repo.dart';
import '../data/create_meal_item_request.dart';

final mealsRepoProvider = Provider<MealsRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return MealsRepo(api, store);
});

class MealItemCreateState {
  final bool isSubmitting;
  final String? error;

  const MealItemCreateState({
    this.isSubmitting = false,
    this.error,
  });

  MealItemCreateState copyWith({
    bool? isSubmitting,
    String? error,
  }) {
    return MealItemCreateState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

class MealItemCreateController extends Notifier<MealItemCreateState> {
  late final MealsRepo _repo;

  @override
  MealItemCreateState build() {
    _repo = ref.read(mealsRepoProvider);
    return const MealItemCreateState();
  }

  Future<bool> submit({
    required String mealId,
    required String groceryId,
    required double quantity,
  }) async {
    if (quantity <= 0) {
      state = state.copyWith(error: 'Quantity must be greater than 0.');
      return false;
    }

    state = state.copyWith(isSubmitting: true, error: null);

    try {
      final req = CreateMealItemRequest(
        groceryId: groceryId,
        quantity: quantity,
      );

      await _repo.createMealItem(mealId: mealId, request: req);

      state = state.copyWith(isSubmitting: false, error: null);
      return true;
    } catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: e.toString(),
      );
      return false;
    }
  }
}

final mealItemCreateControllerProvider =
NotifierProvider.autoDispose<MealItemCreateController, MealItemCreateState>(
  MealItemCreateController.new,
);
