import 'package:flutter_riverpod/flutter_riverpod.dart';

class SharedAchievementsController extends Notifier<Set<String>> {
  @override
  Set<String> build() => <String>{};

  bool isShared(String userAchievementId) => state.contains(userAchievementId);

  void markShared(String userAchievementId) {
    state = {...state, userAchievementId};
  }

  void clear() {
    state = <String>{};
  }
}

final sharedAchievementsProvider =
NotifierProvider<SharedAchievementsController, Set<String>>(
  SharedAchievementsController.new,
);
