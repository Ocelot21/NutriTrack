import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../social/presentation/social_providers.dart';
import 'daily_overview_share_providers.dart';

class DailyOverviewShareSheet extends ConsumerStatefulWidget {
  /// Route parameter for the backend: yyyy-MM-dd (DateOnly)
  final String localDate;
  final String dateLabel;

  const DailyOverviewShareSheet({
    super.key,
    required this.localDate,
    required this.dateLabel,
  });

  @override
  ConsumerState<DailyOverviewShareSheet> createState() =>
      _DailyOverviewShareSheetState();
}

class _DailyOverviewShareSheetState extends ConsumerState<DailyOverviewShareSheet> {
  bool _isSharing = false;
  final _captionController = TextEditingController();

  @override
  void dispose() {
    _captionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final sharedSet = ref.watch(sharedDailyOverviewsProvider);
    final isShared = sharedSet.contains(widget.localDate);

    return SafeArea(
      child: Padding(
        padding: EdgeInsets.only(
          left: 16,
          right: 16,
          top: 16,
          bottom: 16 + MediaQuery.of(context).viewInsets.bottom,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text('Daily overview', style: theme.textTheme.titleMedium),
                const Spacer(),
                IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: 8),

            Text(
              widget.dateLabel,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 16),

            TextField(
              controller: _captionController,
              maxLines: 3,
              decoration: const InputDecoration(
                labelText: 'Caption (optional)',
                hintText: 'How did your day go?',
                border: OutlineInputBorder(),
              ),
            ),

            const SizedBox(height: 16),

            if (isShared)
              Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(vertical: 12),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(12),
                  color: theme.colorScheme.surfaceContainerHighest,
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(Icons.check_circle, color: theme.colorScheme.primary),
                    const SizedBox(width: 8),
                    Text(
                      'Shared',
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              )
            else
              InkWell(
                borderRadius: BorderRadius.circular(12),
                onTap: _isSharing
                    ? null
                    : () async {
                        setState(() => _isSharing = true);

                        try {
                          final caption = _captionController.text.trim();
                          await ref
                              .read(socialRepoProvider)
                              .shareDailyOverviewSnapshot(
                                localDate: widget.localDate,
                                caption: caption.isEmpty ? null : caption,
                                visibility: null,
                              );

                          ref
                              .read(sharedDailyOverviewsProvider.notifier)
                              .markShared(widget.localDate);

                          if (context.mounted) Navigator.of(context).pop();
                        } catch (e) {
                          if (context.mounted) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('Share failed: $e')),
                            );
                          }
                        } finally {
                          if (mounted) setState(() => _isSharing = false);
                        }
                      },
                child: Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(12),
                    color: theme.colorScheme.primary,
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      if (_isSharing) ...[
                        const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                        const SizedBox(width: 10),
                        Text(
                          'Sharing...',
                          style: theme.textTheme.titleSmall?.copyWith(
                            color: theme.colorScheme.onPrimary,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ] else ...[
                        Icon(Icons.share, color: theme.colorScheme.onPrimary),
                        const SizedBox(width: 8),
                        Text(
                          'Share to Social',
                          style: theme.textTheme.titleSmall?.copyWith(
                            color: theme.colorScheme.onPrimary,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ),

            const SizedBox(height: 10),
          ],
        ),
      ),
    );
  }
}

