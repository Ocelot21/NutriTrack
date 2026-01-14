import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/social_models.dart';
import '../data/social_repo.dart';

final socialRepoProvider = Provider<SocialRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return SocialRepo(api, store);
});

class SocialFeedState {
  final bool isLoading;
  final String? error;

  final List<SocialPostModel> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const SocialFeedState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  SocialFeedState copyWith({
    bool? isLoading,
    String? error,
    List<SocialPostModel>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return SocialFeedState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class SocialFeedController extends Notifier<SocialFeedState> {
  late final SocialRepo _repo;

  @override
  SocialFeedState build() {
    _repo = ref.read(socialRepoProvider);
    return const SocialFeedState();
  }

  Future<void> loadInitialIfEmpty() async {
    if (state.items.isNotEmpty || state.isLoading) return;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: 1, append: false);
  }

  Future<void> refresh() async {
    if (state.isLoading) return;
    state = state.copyWith(isLoading: true, error: null, page: 1);
    await _loadPage(page: 1, append: false);
  }

  Future<void> loadMore() async {
    if (!state.canLoadMore || state.isLoading) return;

    final nextPage = state.page + 1;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: nextPage, append: true);
  }

  Future<void> createTextPost(String text) async {
    final trimmed = text.trim();
    if (trimmed.isEmpty) return;

    try {
      await _repo.createTextPost(text: trimmed);
      await refresh();
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  Future<void> shareAchievement(String userAchievementId, {String? caption}) async {
    try {
      await _repo.shareAchievement(
        userAchievementId: userAchievementId,
        caption: caption?.trim().isEmpty == true ? null : caption?.trim(),
      );
      await refresh();
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  Future<void> deletePost(String postId) async {
    try {
      await _repo.deletePost(postId: postId);
      await refresh();
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  Future<void> _loadPage({
    required int page,
    required bool append,
  }) async {
    try {
      final result = await _repo.getFeed(
        page: page,
        pageSize: state.pageSize,
      );

      final newItems = append ? [...state.items, ...result.items] : result.items;

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
        error: 'Unexpected error: $e',
      );
    }
  }
}

final socialFeedControllerProvider =
    NotifierProvider.autoDispose<SocialFeedController, SocialFeedState>(
  SocialFeedController.new,
);

class SocialProfileState {
  final bool isLoading;
  final String? error;
  final SocialProfileModel? profile;

  const SocialProfileState({
    this.isLoading = false,
    this.error,
    this.profile,
  });

  SocialProfileState copyWith({
    bool? isLoading,
    String? error,
    SocialProfileModel? profile,
  }) {
    return SocialProfileState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      profile: profile ?? this.profile,
    );
  }
}

class SocialProfileController extends Notifier<SocialProfileState> {
  late final SocialRepo _repo;

  @override
  SocialProfileState build() {
    _repo = ref.read(socialRepoProvider);
    return const SocialProfileState();
  }

  Future<void> load(String userId) async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      final profile = await _repo.getProfile(userId: userId);
      state = state.copyWith(isLoading: false, profile: profile, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }
}

final socialProfileControllerProvider =
    NotifierProvider.autoDispose<SocialProfileController, SocialProfileState>(
  SocialProfileController.new,
);
