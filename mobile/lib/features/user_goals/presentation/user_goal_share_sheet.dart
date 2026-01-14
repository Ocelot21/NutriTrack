import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../social/presentation/social_providers.dart';
import '../data/user_goal_models.dart';
import 'user_goal_share_providers.dart';

class GoalProgressShareSheet extends ConsumerStatefulWidget {
  final UserGoalDto goal;

  const GoalProgressShareSheet({
    super.key,
    required this.goal,
  });

  @override
  ConsumerState<GoalProgressShareSheet> createState() =>
      _GoalProgressShareSheetState();
}

class _GoalProgressShareSheetState extends ConsumerState<GoalProgressShareSheet> {
  bool _isSharing = false;
  final _captionController = TextEditingController();

  @override
  void dispose() {
    _captionController.dispose();
    super.dispose();
  }

  String _fmtDate(DateTime dt) => dt.toLocal().toString().split(' ').first;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final sharedSet = ref.watch(sharedGoalsProvider);
    final isShared = sharedSet.contains(widget.goal.id);

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
                Text('Goal progress', style: theme.textTheme.titleMedium),
                const Spacer(),
                IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: 8),

            // Summary
            Text(
              widget.goal.type.label,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                Chip(
                  label: Text(
                    'Start ${widget.goal.startWeightKg.toStringAsFixed(1)} kg',
                  ),
                ),
                Chip(
                  label: Text(
                    'Target ${widget.goal.targetWeightKg.toStringAsFixed(1)} kg',
                  ),
                ),
                Chip(
                  label: Text(
                    '${_fmtDate(widget.goal.startDate)} → ${_fmtDate(widget.goal.targetDate)}',
                  ),
                ),
              ],
            ),

            const SizedBox(height: 16),

            TextField(
              controller: _captionController,
              maxLines: 3,
              decoration: const InputDecoration(
                labelText: 'Caption (optional)',
                hintText: 'Say something about your progress…',
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
                              .shareGoalProgressSnapshot(
                                userGoalId: widget.goal.id,
                                caption: caption.isEmpty ? null : caption,
                                visibility: null,
                              );

                          ref
                              .read(sharedGoalsProvider.notifier)
                              .markShared(widget.goal.id);

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

