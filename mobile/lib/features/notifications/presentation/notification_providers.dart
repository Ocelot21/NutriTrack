import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/notification_models.dart';
import '../data/notification_repo.dart';

final notificationRepoProvider = Provider<NotificationRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return NotificationRepo(api, store);
});

class NotificationListState {
  final bool isLoading;
  final String? error;
  final List<AppNotification> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final bool onlyUnread;

  bool get canLoadMore => items.length < totalCount;

  const NotificationListState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 10,
    this.onlyUnread = false,
  });

  NotificationListState copyWith({
    bool? isLoading,
    String? error,
    List<AppNotification>? items,
    int? totalCount,
    int? page,
    int? pageSize,
    bool? onlyUnread,
  }) {
    return NotificationListState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
      onlyUnread: onlyUnread ?? this.onlyUnread,
    );
  }
}

class NotificationListController extends Notifier<NotificationListState> {
  late final NotificationRepo _repo;

  @override
  NotificationListState build() {
    _repo = ref.read(notificationRepoProvider);
    return const NotificationListState();
  }

  Future<void> loadInitial({bool refresh = false}) async {
    if (state.items.isNotEmpty && !refresh) return;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: 1, append: false);
  }

  Future<void> toggleUnreadFilter(bool onlyUnread) async {
    state = state.copyWith(
      onlyUnread: onlyUnread,
      isLoading: true,
      items: [],
      totalCount: 0,
      page: 1,
      error: null,
    );
    await _loadPage(page: 1, append: false);
  }

  Future<void> loadMore() async {
    if (!state.canLoadMore || state.isLoading) return;
    final nextPage = state.page + 1;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: nextPage, append: true);
  }

  Future<void> markAsRead(String id) async {
    await _repo.markAsRead(id);
    final updated = state.items.map((n) {
      if (n.id == id) {
        return AppNotification(
          id: n.id,
          title: n.title,
          message: n.message,
          type: n.type,
          status: NotificationStatusUi.read,
          occurredAt: n.occurredAt,
          readAt: n.readAt ?? DateTime.now(),
          linkUrl: n.linkUrl,
          metadataJson: n.metadataJson,
        );
      }
      return n;
    }).toList();

    state = state.copyWith(items: updated);

    // Invalidate unread count to refresh badge
    ref.invalidate(unreadNotificationCountProvider);
  }

  Future<void> markAllAsRead() async {
    await _repo.markAllAsRead();
    final updated = state.items
        .map((n) => AppNotification(
      id: n.id,
      title: n.title,
      message: n.message,
      type: n.type,
      status: NotificationStatusUi.read,
      occurredAt: n.occurredAt,
      readAt: n.readAt ?? DateTime.now(),
      linkUrl: n.linkUrl,
      metadataJson: n.metadataJson,
    ))
        .toList();

    state = state.copyWith(items: updated);

    // Invalidate unread count to refresh badge
    ref.invalidate(unreadNotificationCountProvider);
  }

  Future<void> _loadPage({
    required int page,
    required bool append,
  }) async {
    try {
      final result = await _repo.getNotifications(
        page: page,
        pageSize: state.pageSize,
        onlyUnread: state.onlyUnread,
      );

      final newItems = append
          ? [...state.items, ...result.items]
          : result.items;

      state = state.copyWith(
        isLoading: false,
        items: newItems,
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
        error: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }
}

final notificationListControllerProvider =
NotifierProvider.autoDispose<NotificationListController, NotificationListState>(
  NotificationListController.new,
);

// Unread count provider with auto-refresh
final unreadNotificationCountProvider = StreamProvider.autoDispose<int>((ref) async* {
  final repo = ref.read(notificationRepoProvider);

  // Initial fetch
  yield await repo.getUnreadCount();

  // Refresh every 30 seconds
  await for (final _ in Stream.periodic(const Duration(seconds: 30))) {
    yield await repo.getUnreadCount();
  }
});

