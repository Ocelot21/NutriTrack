import 'package:flutter_riverpod/flutter_riverpod.dart';

class SharedGoalsController extends Notifier<Set<String>> {
  @override
  Set<String> build() => <String>{};

  bool isShared(String userGoalId) => state.contains(userGoalId);

  void markShared(String userGoalId) {
    state = {...state, userGoalId};
  }

  void clear() {
    state = <String>{};
  }
}

final sharedGoalsProvider = NotifierProvider<SharedGoalsController, Set<String>>(
  SharedGoalsController.new,
);

