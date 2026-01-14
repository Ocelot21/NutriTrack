import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Tracks which daily overviews (by local date string yyyy-MM-dd) have been shared
/// during this app session.
class SharedDailyOverviewsController extends Notifier<Set<String>> {
  @override
  Set<String> build() => <String>{};

  bool isShared(String localDate) => state.contains(localDate);

  void markShared(String localDate) {
    state = {...state, localDate};
  }

  void clear() {
    state = <String>{};
  }
}

final sharedDailyOverviewsProvider =
    NotifierProvider<SharedDailyOverviewsController, Set<String>>(
  SharedDailyOverviewsController.new,
);

