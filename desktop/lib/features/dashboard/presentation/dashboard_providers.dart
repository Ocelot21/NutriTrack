import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/dashboard_models.dart';
import '../data/dashboard_repo.dart';

final dashboardRepoProvider = Provider<DashboardRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return DashboardRepo(api, store);
});

class DashboardSummaryState {
  final bool isLoading;
  final String? error;
  final AdminDashboardSummaryResponse? summary;
  final GetAdminDashboardSummaryRequest request;

  const DashboardSummaryState({
    this.isLoading = false,
    this.error,
    this.summary,
    this.request = const GetAdminDashboardSummaryRequest(),
  });

  DashboardSummaryState copyWith({
    bool? isLoading,
    String? error,
    AdminDashboardSummaryResponse? summary,
    GetAdminDashboardSummaryRequest? request,
  }) {
    return DashboardSummaryState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      summary: summary ?? this.summary,
      request: request ?? this.request,
    );
  }
}

class DashboardSummaryController extends Notifier<DashboardSummaryState> {
  late final DashboardRepo _repo;

  @override
  DashboardSummaryState build() {
    _repo = ref.read(dashboardRepoProvider);

    // fire-and-forget initial load
    Future.microtask(load);

    return const DashboardSummaryState();
  }

  Future<void> setRequest(GetAdminDashboardSummaryRequest request) async {
    state = state.copyWith(request: request);
    await load();
  }

  Future<void> refresh() => load();

  Future<void> load() async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      final res = await _repo.getSummary(
        days: state.request.days,
        top: state.request.top,
      );
      state = state.copyWith(isLoading: false, summary: res, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }
}

final dashboardSummaryControllerProvider =
    NotifierProvider<DashboardSummaryController, DashboardSummaryState>(
  DashboardSummaryController.new,
);

// NOTE: Legacy FutureProvider removed in favor of DashboardSummaryController.
