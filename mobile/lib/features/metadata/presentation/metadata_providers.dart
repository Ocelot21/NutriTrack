import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import 'package:nutritrack_shared/metadata/data/metadata_repo.dart';
import 'package:nutritrack_shared/metadata/data/metadata_models.dart';

/// Repo provider
final metadataRepoProvider = Provider<MetadataRepo>((ref) {
  final api = ref.read(apiClientProvider);
  return MetadataRepo(api);
});

/// Countries – for dropdown / picker
final countriesProvider =
FutureProvider<List<CountrySummary>>((ref) async {
  final repo = ref.read(metadataRepoProvider);
  return repo.getCountries();
});

/// Time zones – for dropdown / picker
final timeZonesProvider = FutureProvider<List<String>>((ref) async {
  final repo = ref.read(metadataRepoProvider);
  return repo.getTimeZones();
});
