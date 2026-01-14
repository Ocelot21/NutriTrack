import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';

import '../data/daily_overview_models.dart';
import '../data/daily_overview_repo.dart';

final dailyOverviewRepoProvider = Provider<DailyOverviewRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return DailyOverviewRepo(api, store);
});

class DailyOverviewState {
  final bool isLoading;
  final DailyOverview? overview;
  final String? error;

  const DailyOverviewState({
    this.isLoading = false,
    this.overview,
    this.error,
  });

  DailyOverviewState copyWith({
    bool? isLoading,
    DailyOverview? overview,
    String? error,
  }) {
    return DailyOverviewState(
      isLoading: isLoading ?? this.isLoading,
      overview: overview ?? this.overview,
      error: error,
    );
  }
}

class DailyOverviewController extends Notifier<DailyOverviewState> {
  late final DailyOverviewRepo _repo;

  @override
  DailyOverviewState build() {
    _repo = ref.read(dailyOverviewRepoProvider);
    return const DailyOverviewState();
  }

  Future<void> loadForDate(
      DateTime date, {
        void Function()? onHealthProfileNotCompleted,
      }) async {
    state = state.copyWith(isLoading: true, error: null);

    try {
      final overview = await _repo.getDailyOverview(date);
      state = state.copyWith(isLoading: false, overview: overview, error: null);
    } on HealthProfileNotCompletedException {
      state = state.copyWith(isLoading: false, error: null);

      if (onHealthProfileNotCompleted != null) {
        onHealthProfileNotCompleted();
      }
    } on DailyOverviewException catch (e) {
      state = state.copyWith(isLoading: false, error: e.message);
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: 'Unexpected error: $e',
      );
    }
  }

  Future<void> loadToday({void Function()? onHealthProfileNotCompleted}) {
    final today = DateTime.now();
    final dateOnly = DateTime(today.year, today.month, today.day);
    return loadForDate(
      dateOnly,
      onHealthProfileNotCompleted: onHealthProfileNotCompleted,
    );
  }
}

final dailyOverviewControllerProvider =
NotifierProvider<DailyOverviewController, DailyOverviewState>(
  DailyOverviewController.new,
);
