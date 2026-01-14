import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/report_models.dart';
import '../data/report_repo.dart';

final reportsRepoProvider = Provider<ReportsRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return ReportsRepo(api, store);
});

// -----------------------------------------------------------------------------
// Runs list (paged)
// -----------------------------------------------------------------------------

class ReportRunsListState {
  final bool isLoading;
  final String? error;
  final List<ReportRunModel> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canPrev => page > 1;
  bool get canNext => items.length + ((page - 1) * pageSize) < totalCount;

  const ReportRunsListState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  ReportRunsListState copyWith({
    bool? isLoading,
    String? error,
    List<ReportRunModel>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return ReportRunsListState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class ReportRunsListController extends Notifier<ReportRunsListState> {
  late final ReportsRepo _repo;

  @override
  ReportRunsListState build() {
    _repo = ref.read(reportsRepoProvider);
    return const ReportRunsListState();
  }

  Future<void> loadInitialIfEmpty() async {
    if (state.isLoading || state.items.isNotEmpty) return;
    await loadPage(1);
  }

  Future<void> refresh() async {
    state = state.copyWith(isLoading: true, error: null);
    await _load(page: state.page, pageSize: state.pageSize);
  }

  Future<void> setPageSize(int pageSize) async {
    state = state.copyWith(
      isLoading: true,
      error: null,
      pageSize: pageSize,
      page: 1,
      items: const [],
      totalCount: 0,
    );
    await _load(page: 1, pageSize: pageSize);
  }

  Future<void> loadPage(int page) async {
    state = state.copyWith(isLoading: true, error: null, page: page);
    await _load(page: page, pageSize: state.pageSize);
  }

  Future<void> nextPage() async {
    if (!state.canNext || state.isLoading) return;
    await loadPage(state.page + 1);
  }

  Future<void> prevPage() async {
    if (!state.canPrev || state.isLoading) return;
    await loadPage(state.page - 1);
  }

  Future<void> _load({required int page, required int pageSize}) async {
    try {
      final res = await _repo.listRuns(page: page, pageSize: pageSize);
      state = state.copyWith(
        isLoading: false,
        error: null,
        items: res.items,
        totalCount: res.totalCount,
        page: res.page,
        pageSize: res.pageSize,
      );
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }
}

final reportRunsListControllerProvider =
    NotifierProvider<ReportRunsListController, ReportRunsListState>(
  ReportRunsListController.new,
);

// -----------------------------------------------------------------------------
// Run details
// -----------------------------------------------------------------------------

final reportRunDetailsProvider =
    FutureProvider.autoDispose.family<ReportRunModel, String>((ref, id) async {
  final repo = ref.read(reportsRepoProvider);
  return repo.getRun(id: id);
});

// -----------------------------------------------------------------------------
// Create run action
// -----------------------------------------------------------------------------

class CreateReportRunState {
  final bool isLoading;
  final String? error;
  final ReportRunModel? created;

  const CreateReportRunState({
    this.isLoading = false,
    this.error,
    this.created,
  });

  CreateReportRunState copyWith({
    bool? isLoading,
    String? error,
    ReportRunModel? created,
  }) {
    return CreateReportRunState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      created: created,
    );
  }
}

class CreateReportRunController extends Notifier<CreateReportRunState> {
  late final ReportsRepo _repo;

  @override
  CreateReportRunState build() {
    _repo = ref.read(reportsRepoProvider);
    return const CreateReportRunState();
  }

  Future<ReportRunModel?> create(CreateReportRunRequestModel request) async {
    state = state.copyWith(isLoading: true, error: null, created: null);
    try {
      final run = await _repo.createRun(request);
      state = state.copyWith(isLoading: false, created: run, error: null);
      return run;
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      return null;
    }
  }

  void reset() {
    state = const CreateReportRunState();
  }
}

final createReportRunControllerProvider =
    NotifierProvider<CreateReportRunController, CreateReportRunState>(
  CreateReportRunController.new,
);

// -----------------------------------------------------------------------------
// PDF URL helper
// -----------------------------------------------------------------------------

final reportPdfUrlProvider = FutureProvider.autoDispose.family<Uri, String>((ref, id) async {
  final repo = ref.read(reportsRepoProvider);
  return repo.getDownloadPdfUrl(id: id);
});

