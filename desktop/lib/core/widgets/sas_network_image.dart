import 'package:flutter/material.dart';

/// Displays a network image safely, including SAS URLs containing '&' etc.
///
/// This widget adds:
/// - URI validation
/// - loading indicator
/// - consistent fallback UI when the URL is missing/invalid/unreachable
class SasNetworkImage extends StatelessWidget {
  final String? url;
  final BoxFit fit;
  final BorderRadius? borderRadius;

  const SasNetworkImage({
    super.key,
    required this.url,
    this.fit = BoxFit.cover,
    this.borderRadius,
  });

  @override
  Widget build(BuildContext context) {
    final trimmed = url?.trim();
    final parsed = (trimmed == null || trimmed.isEmpty) ? null : Uri.tryParse(trimmed);
    final isHttp = parsed != null && (parsed.scheme == 'http' || parsed.scheme == 'https');

    Widget child;

    if (!isHttp) {
      child = _placeholder(context);
    } else {
      child = Image.network(
        parsed.toString(),
        fit: fit,
        // Avoid infinite spinners by showing progress.
        loadingBuilder: (context, w, progress) {
          if (progress == null) return w;
          final expected = progress.expectedTotalBytes;
          final loaded = progress.cumulativeBytesLoaded;
          final value = expected == null ? null : loaded / expected;
          return Center(
            child: SizedBox(
              width: 22,
              height: 22,
              child: CircularProgressIndicator(strokeWidth: 2, value: value),
            ),
          );
        },
        errorBuilder: (context, error, stack) => _broken(context),
      );
    }

    if (borderRadius != null) {
      return ClipRRect(borderRadius: borderRadius!, child: child);
    }
    return child;
  }

  Widget _placeholder(BuildContext context) {
    return Container(
      color: Theme.of(context).colorScheme.surfaceContainerHighest,
      child: const Center(
        child: Icon(Icons.image_not_supported, size: 40),
      ),
    );
  }

  Widget _broken(BuildContext context) {
    return Container(
      color: Theme.of(context).colorScheme.surfaceContainerHighest,
      child: const Center(
        child: Icon(Icons.broken_image, size: 40),
      ),
    );
  }
}

