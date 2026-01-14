import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/report_models.dart';
import 'report_providers.dart';

class ReportsPage extends ConsumerStatefulWidget {
  const ReportsPage({super.key});

  @override
  ConsumerState<ReportsPage> createState() => _ReportsPageState();
}

class _ReportsPageState extends ConsumerState<ReportsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() =>
        ref.read(reportRunsListControllerProvider.notifier).loadInitialIfEmpty());
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final state = ref.watch(reportRunsListControllerProvider);
    final controller = ref.read(reportRunsListControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Reports'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            onPressed: state.isLoading ? null : controller.refresh,
            icon: const Icon(Icons.refresh),
          ),
          const SizedBox(width: 8),
          FilledButton.icon(
            onPressed: state.isLoading
                ? null
                : () async {
                    final created = await showDialog<ReportRunModel?>(
                      context: context,
                      builder: (_) => const _CreateReportRunDialog(),
                    );
                    if (created != null) {
                      controller.refresh();
                      if (context.mounted) {
                        await showDialog<void>(
                          context: context,
                          builder: (_) => _ReportRunDetailsDialog(runId: created.id),
                        );
                      }
                    }
                  },
            icon: const Icon(Icons.add),
            label: const Text('New report run'),
          ),
          const SizedBox(width: 12),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    'Report runs',
                    style: theme.textTheme.titleLarge,
                  ),
                ),
                SizedBox(
                  width: 220,
                  child: DropdownButtonFormField<int>(
                    decoration: const InputDecoration(
                      labelText: 'Page size',
                      isDense: true,
                    ),
                    initialValue: state.pageSize,
                    items: const [10, 20, 50, 100]
                        .map((v) => DropdownMenuItem(value: v, child: Text('$v')))
                        .toList(growable: false),
                    onChanged: state.isLoading
                        ? null
                        : (v) {
                            if (v == null) return;
                            controller.setPageSize(v);
                          },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            if (state.error != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: _ErrorBanner(
                  message: state.error!,
                  onRetry: state.isLoading ? null : controller.refresh,
                ),
              ),

            Expanded(
              child: Card(
                elevation: 2,
                child: state.isLoading && state.items.isEmpty
                    ? const Center(child: CircularProgressIndicator())
                    : state.items.isEmpty
                        ? const Center(child: Text('No report runs yet.'))
                        : SingleChildScrollView(
                            child: DataTable(
                              showCheckboxColumn: false,
                              columns: const [
                                DataColumn(label: Text('Type')),
                                DataColumn(label: Text('Status')),
                                DataColumn(label: Text('Requested')),
                                DataColumn(label: Text('Range')),
                                DataColumn(label: Text('Output file')),
                                DataColumn(label: Text('Actions')),
                              ],
                              rows: state.items
                                  .map((r) => _row(context, theme, r))
                                  .toList(growable: false),
                            ),
                          ),
              ),
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Total: ${state.totalCount}  •  Page ${state.page}',
                  style: theme.textTheme.labelLarge,
                ),
                Row(
                  children: [
                    OutlinedButton.icon(
                      onPressed: state.isLoading || !state.canPrev
                          ? null
                          : controller.prevPage,
                      icon: const Icon(Icons.chevron_left),
                      label: const Text('Prev'),
                    ),
                    const SizedBox(width: 8),
                    OutlinedButton.icon(
                      onPressed: state.isLoading || !state.canNext
                          ? null
                          : controller.nextPage,
                      icon: const Icon(Icons.chevron_right),
                      label: const Text('Next'),
                    ),
                  ],
                ),
              ],
            )
          ],
        ),
      ),
    );
  }

  DataRow _row(BuildContext context, ThemeData theme, ReportRunModel r) {
    return DataRow(
      onSelectChanged: (_) => _openDetails(context, r.id),
      cells: [
        DataCell(Text(reportTypeLabel(r.type))),
        DataCell(_StatusChip(status: r.status)),
        DataCell(Text(_fmtDateTimeUtc(r.requestedAtUtc))),
        DataCell(Text('${_fmtDateOnly(r.fromUtc)} → ${_fmtDateOnly(r.toUtc)}')),
        DataCell(Text(r.outputFileName ?? '-')),
        DataCell(
          Row(
            children: [
              IconButton(
                tooltip: 'Details',
                icon: const Icon(Icons.info_outline),
                onPressed: () => _openDetails(context, r.id),
              ),
              IconButton(
                tooltip: 'Open PDF',
                icon: const Icon(Icons.picture_as_pdf_outlined),
                onPressed: (r.status == ReportRunStatus.ready && r.isPdfAvailable)
                    ? () => _openPdf(context, r: r)
                    : null,
              ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _openDetails(BuildContext context, String id) async {
    await showDialog<void>(
      context: context,
      builder: (_) => _ReportRunDetailsDialog(runId: id),
    );
  }

  Future<void> _openPdf(BuildContext context, {required ReportRunModel r}) async {
    try {
      // Prefer SAS URL directly from the row payload.
      final uriStr = r.outputPdfUri;
      if (uriStr != null && uriStr.isNotEmpty) {
        final parsed = Uri.tryParse(uriStr);
        if (parsed != null) {
          await _openUrlExternal(parsed);
          return;
        }
      }

      // Fallback: hit the /pdf endpoint (may require auth).
      final url = await ref.read(reportPdfUrlProvider(r.id).future);
      await _openUrlExternal(url);
    } catch (e) {
      if (!context.mounted) return;
      await showDialog<void>(
        context: context,
        builder: (_) => AlertDialog(
          title: const Text('PDF not available'),
          content: Text(e.toString()),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('OK'),
            )
          ],
        ),
      );
    }
  }
}

class _CreateReportRunDialog extends ConsumerStatefulWidget {
  const _CreateReportRunDialog();

  @override
  ConsumerState<_CreateReportRunDialog> createState() => _CreateReportRunDialogState();
}

class _CreateReportRunDialogState extends ConsumerState<_CreateReportRunDialog> {
  ReportType _type = ReportType.weeklyOverview;

  late DateTime _fromUtc;
  late DateTime _toUtc;

  final _parametersController = TextEditingController();
  String? _error;

  @override
  void initState() {
    super.initState();

    final now = DateTime.now().toUtc();
    _toUtc = DateTime.utc(now.year, now.month, now.day);
    _fromUtc = _toUtc.subtract(const Duration(days: 7));
  }

  @override
  void dispose() {
    _parametersController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final createState = ref.watch(createReportRunControllerProvider);
    final controller = ref.read(createReportRunControllerProvider.notifier);

    return AlertDialog(
      title: const Text('Create report run'),
      content: SizedBox(
        width: 640,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (_error != null) ...[
              Text(
                _error!,
                style: theme.textTheme.bodyMedium
                    ?.copyWith(color: theme.colorScheme.error),
              ),
              const SizedBox(height: 12),
            ],
            if (createState.error != null) ...[
              Text(
                createState.error!,
                style: theme.textTheme.bodyMedium
                    ?.copyWith(color: theme.colorScheme.error),
              ),
              const SizedBox(height: 12),
            ],
            DropdownButtonFormField<ReportType>(
              decoration: const InputDecoration(labelText: 'Type'),
              initialValue: _type,
              items: ReportType.values
                  .map(
                    (t) => DropdownMenuItem(
                      value: t,
                      child: Text(reportTypeLabel(t)),
                    ),
                  )
                  .toList(growable: false),
              onChanged: createState.isLoading
                  ? null
                  : (v) {
                      if (v == null) return;
                      setState(() => _type = v);
                    },
            ),
            const SizedBox(height: 12),
            _UtcDatePicker(
              label: 'From (UTC)',
              value: _fromUtc,
              enabled: !createState.isLoading,
              onChanged: (v) => setState(() => _fromUtc = v),
            ),
            const SizedBox(height: 12),
            _UtcDatePicker(
              label: 'To (UTC)',
              value: _toUtc,
              enabled: !createState.isLoading,
              onChanged: (v) => setState(() => _toUtc = v),
            ),
            const SizedBox(height: 12),
            ExpansionTile(
              tilePadding: EdgeInsets.zero,
              title: const Text('Parameters (optional)'),
              children: [
                TextField(
                  controller: _parametersController,
                  minLines: 3,
                  maxLines: 6,
                  enabled: !createState.isLoading,
                  decoration: const InputDecoration(
                    hintText: '{\n  "foo": "bar"\n}',
                    border: OutlineInputBorder(),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: createState.isLoading
              ? null
              : () {
                  controller.reset();
                  Navigator.of(context).pop(null);
                },
          child: const Text('Cancel'),
        ),
        FilledButton(
          onPressed: createState.isLoading
              ? null
              : () async {
                  setState(() => _error = null);

                  if (!_fromUtc.isBefore(_toUtc)) {
                    setState(() => _error = 'From must be earlier than To.');
                    return;
                  }

                  Map<String, dynamic>? params;
                  final txt = _parametersController.text.trim();
                  if (txt.isNotEmpty) {
                    try {
                      final decoded = jsonDecode(txt);
                      if (decoded is Map<String, dynamic>) {
                        params = decoded;
                      } else {
                        setState(() => _error = 'Parameters must be a JSON object.');
                        return;
                      }
                    } catch (_) {
                      setState(() => _error = 'Invalid JSON in parameters.');
                      return;
                    }
                  }

                  final run = await controller.create(
                    CreateReportRunRequestModel(
                      type: _type,
                      fromUtc: _fromUtc,
                      toUtc: _toUtc,
                      parameters: params,
                    ),
                  );

                  if (!context.mounted) return;

                  if (run != null) {
                    controller.reset();
                    Navigator.of(context).pop(run);
                  }
                },
          child: createState.isLoading
              ? const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Create'),
        ),
      ],
    );
  }
}

class _ReportRunDetailsDialog extends ConsumerWidget {
  final String runId;

  const _ReportRunDetailsDialog({required this.runId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final asyncRun = ref.watch(reportRunDetailsProvider(runId));

    return AlertDialog(
      title: const Text('Report run details'),
      content: SizedBox(
        width: 780,
        child: asyncRun.when(
          loading: () => const SizedBox(
            height: 160,
            child: Center(child: CircularProgressIndicator()),
          ),
          error: (e, _) => _ErrorBanner(
            message: e.toString(),
            onRetry: () => ref.invalidate(reportRunDetailsProvider(runId)),
          ),
          data: (run) {
            return Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // NOTE: Intentionally not showing internal run ID.
                _kv('Type', reportTypeLabel(run.type), theme),
                _kv('Status', reportRunStatusLabel(run.status), theme),
                _kv('Requested at (UTC)', _fmtDateTimeUtc(run.requestedAtUtc), theme),
                _kv('From (UTC)', _fmtDateTimeUtc(run.fromUtc.toUtc()), theme),
                _kv('To (UTC)', _fmtDateTimeUtc(run.toUtc.toUtc()), theme),
                _kv('Output file', run.outputFileName ?? '-', theme),
                // Prefer only the SAS URL field (public) for display.
                _kv('Download URL', run.outputPdfUri ?? '-', theme),
                _kv('Started at (UTC)', run.startedAtUtc == null ? '-' : _fmtDateTimeUtc(run.startedAtUtc!), theme),
                _kv('Completed at (UTC)', run.completedAtUtc == null ? '-' : _fmtDateTimeUtc(run.completedAtUtc!), theme),
                _kv('Failure reason', run.failureReason ?? '-', theme),
              ],
            );
          },
        ),
      ),
      actions: [
        asyncRun.maybeWhen(
          data: (run) {
            final canOpen = run.status == ReportRunStatus.ready && run.isPdfAvailable;
            return FilledButton.icon(
              onPressed: !canOpen
                  ? null
                  : () async {
                      try {
                        // outputPdfUri is the SAS-generated public URI.
                        final uriStr = run.outputPdfUri;
                        if (uriStr == null || uriStr.isEmpty) {
                          throw Exception('No download URL available yet.');
                        }
                        await _openUrlExternal(Uri.parse(uriStr));
                      } catch (e) {
                        if (!context.mounted) return;
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(content: Text(e.toString())),
                        );
                      }
                    },
              icon: const Icon(Icons.picture_as_pdf_outlined),
              label: const Text('Open PDF'),
            );
          },
          orElse: () => const SizedBox.shrink(),
        ),
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Close'),
        ),
      ],
    );
  }

  Widget _kv(String k, String v, ThemeData theme) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 170,
            child: Text(
              k,
              style: theme.textTheme.labelLarge,
            ),
          ),
          Expanded(
            child: SelectableText(
              v,
              style: theme.textTheme.bodyMedium,
            ),
          ),
        ],
      ),
    );
  }
}

class _StatusChip extends StatelessWidget {
  final ReportRunStatus status;

  const _StatusChip({required this.status});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    Color bg;
    Color fg;

    switch (status) {
      case ReportRunStatus.queued:
        bg = theme.colorScheme.secondaryContainer;
        fg = theme.colorScheme.onSecondaryContainer;
        break;
      case ReportRunStatus.running:
        bg = theme.colorScheme.primaryContainer;
        fg = theme.colorScheme.onPrimaryContainer;
        break;
      case ReportRunStatus.ready:
        bg = theme.colorScheme.tertiaryContainer;
        fg = theme.colorScheme.onTertiaryContainer;
        break;
      case ReportRunStatus.failed:
        bg = theme.colorScheme.errorContainer;
        fg = theme.colorScheme.onErrorContainer;
        break;
      case ReportRunStatus.unknown:
        bg = theme.colorScheme.surfaceContainerHighest;
        fg = theme.colorScheme.onSurface;
        break;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: bg,
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        reportRunStatusLabel(status),
        style: theme.textTheme.labelMedium?.copyWith(color: fg),
      ),
    );
  }
}

class _UtcDatePicker extends StatelessWidget {
  final String label;
  final DateTime value;
  final bool enabled;
  final ValueChanged<DateTime> onChanged;

  const _UtcDatePicker({
    required this.label,
    required this.value,
    required this.enabled,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InputDecorator(
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
      ),
      child: Row(
        children: [
          Expanded(child: Text(_fmtDateTimeUtc(value), style: theme.textTheme.bodyMedium)),
          const SizedBox(width: 8),
          OutlinedButton.icon(
            onPressed: !enabled
                ? null
                : () async {
                    final picked = await showDatePicker(
                      context: context,
                      initialDate: value.toLocal(),
                      firstDate: DateTime(2000),
                      lastDate: DateTime(2100),
                    );
                    if (picked == null) return;
                    onChanged(DateTime.utc(picked.year, picked.month, picked.day));
                  },
            icon: const Icon(Icons.date_range_outlined),
            label: const Text('Pick'),
          ),
        ],
      ),
    );
  }
}

class _ErrorBanner extends StatelessWidget {
  final String message;
  final VoidCallback? onRetry;

  const _ErrorBanner({required this.message, this.onRetry});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      color: theme.colorScheme.errorContainer,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            Icon(Icons.error_outline, color: theme.colorScheme.onErrorContainer),
            const SizedBox(width: 10),
            Expanded(
              child: Text(
                message,
                style: theme.textTheme.bodyMedium
                    ?.copyWith(color: theme.colorScheme.onErrorContainer),
              ),
            ),
            if (onRetry != null) ...[
              const SizedBox(width: 12),
              TextButton.icon(
                onPressed: onRetry,
                icon: Icon(Icons.refresh, color: theme.colorScheme.onErrorContainer),
                label: Text(
                  'Retry',
                  style: theme.textTheme.labelLarge
                      ?.copyWith(color: theme.colorScheme.onErrorContainer),
                ),
              ),
            ]
          ],
        ),
      ),
    );
  }
}

Future<void> _openUrlExternal(Uri uri) async {
  final url = uri.toString();

  if (Platform.isWindows) {
    // IMPORTANT: `cmd /c start` treats '&' as a command separator, which truncates SAS URLs.
    // Use PowerShell Start-Process with proper quoting so the full query string is preserved.
    await Process.run(
      'powershell',
      [
        '-NoProfile',
        '-Command',
        'Start-Process -FilePath ${_psQuote(url)}',
      ],
    );
    return;
  }

  if (Platform.isMacOS) {
    await Process.run('open', [url]);
    return;
  }

  if (Platform.isLinux) {
    await Process.run('xdg-open', [url]);
    return;
  }

  throw Exception('Unsupported platform for opening URLs: ${Platform.operatingSystem}');
}

String _psQuote(String s) {
  // PowerShell single-quote escaping: '' inside a single-quoted string.
  final escaped = s.replaceAll("'", "''");
  return "'$escaped'";
}

String _fmtDateOnly(DateTime dt) {
  final d = dt.toUtc();
  final y = d.year.toString().padLeft(4, '0');
  final m = d.month.toString().padLeft(2, '0');
  final day = d.day.toString().padLeft(2, '0');
  return '$y-$m-$day';
}

String _fmtDateTimeUtc(DateTime dt) {
  final d = dt.toUtc();
  final y = d.year.toString().padLeft(4, '0');
  final m = d.month.toString().padLeft(2, '0');
  final day = d.day.toString().padLeft(2, '0');
  final hh = d.hour.toString().padLeft(2, '0');
  final mm = d.minute.toString().padLeft(2, '0');
  return '$y-$m-$day $hh:$mm UTC';
}

