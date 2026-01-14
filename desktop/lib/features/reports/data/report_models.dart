// Reports feature models (Admin).
//
// Mirrors backend `ReportsController` contracts (see `lib/features/some.md`).

// -----------------------------------------------------------------------------
// Enums with safe fallback
// -----------------------------------------------------------------------------

/// Report type (server uses an int enum).
enum ReportType {
  weeklyOverview,
  userActivity,
  adminAudit,
  unknown,
}

ReportType reportTypeFromRaw(int raw) {
  switch (raw) {
    case 1:
      return ReportType.weeklyOverview;
    case 2:
      return ReportType.userActivity;
    case 3:
      return ReportType.adminAudit;
    default:
      return ReportType.unknown;
  }
}

int reportTypeToRaw(ReportType type) {
  switch (type) {
    case ReportType.weeklyOverview:
      return 1;
    case ReportType.userActivity:
      return 2;
    case ReportType.adminAudit:
      return 3;
    case ReportType.unknown:
      // Best-effort fallback.
      return 1;
  }
}

String reportTypeLabel(ReportType type) {
  switch (type) {
    case ReportType.weeklyOverview:
      return 'Weekly overview';
    case ReportType.userActivity:
      return 'User activity';
    case ReportType.adminAudit:
      return 'Admin audit';
    case ReportType.unknown:
      return 'Unknown';
  }
}

/// Report run status (server uses an int enum).
enum ReportRunStatus {
  queued,
  running,
  ready,
  failed,
  unknown,
}

ReportRunStatus reportRunStatusFromRaw(int raw) {
  switch (raw) {
    case 1:
      return ReportRunStatus.queued;
    case 2:
      return ReportRunStatus.running;
    case 3:
      return ReportRunStatus.ready;
    case 4:
      return ReportRunStatus.failed;
    default:
      return ReportRunStatus.unknown;
  }
}

String reportRunStatusLabel(ReportRunStatus status) {
  switch (status) {
    case ReportRunStatus.queued:
      return 'Queued';
    case ReportRunStatus.running:
      return 'Running';
    case ReportRunStatus.ready:
      return 'Ready';
    case ReportRunStatus.failed:
      return 'Failed';
    case ReportRunStatus.unknown:
      return 'Unknown';
  }
}

// -----------------------------------------------------------------------------
// DTOs
// -----------------------------------------------------------------------------

class CreateReportRunRequestModel {
  final ReportType type;
  final DateTime fromUtc;
  final DateTime toUtc;

  /// Optional JSON-ish parameters.
  final Map<String, dynamic>? parameters;

  const CreateReportRunRequestModel({
    required this.type,
    required this.fromUtc,
    required this.toUtc,
    this.parameters,
  });

  Map<String, dynamic> toJson() => {
        'type': reportTypeToRaw(type),
        'fromUtc': fromUtc.toUtc().toIso8601String(),
        'toUtc': toUtc.toUtc().toIso8601String(),
        'parameters': parameters,
      };
}

class ReportRunModel {
  final String id;
  final ReportType type;
  final ReportRunStatus status;

  final DateTime requestedAtUtc;
  final DateTime fromUtc;
  final DateTime toUtc;

  final String? outputPdfUri;
  final String? outputPdfBlobName;
  final String? outputFileName;

  final DateTime? startedAtUtc;
  final DateTime? completedAtUtc;

  final String? failureReason;

  const ReportRunModel({
    required this.id,
    required this.type,
    required this.status,
    required this.requestedAtUtc,
    required this.fromUtc,
    required this.toUtc,
    required this.outputPdfUri,
    required this.outputPdfBlobName,
    required this.outputFileName,
    required this.startedAtUtc,
    required this.completedAtUtc,
    required this.failureReason,
  });

  factory ReportRunModel.fromJson(Map<String, dynamic> json) {
    DateTime parseDate(dynamic v) => DateTime.parse(v as String);

    DateTime? parseNullableDate(dynamic v) {
      if (v == null) return null;
      return DateTime.parse(v as String);
    }

    return ReportRunModel(
      id: json['id'] as String,
      type: reportTypeFromRaw((json['type'] as num?)?.toInt() ?? -1),
      status: reportRunStatusFromRaw((json['status'] as num?)?.toInt() ?? -1),
      requestedAtUtc: parseDate(json['requestedAtUtc']).toUtc(),
      fromUtc: parseDate(json['fromUtc']),
      toUtc: parseDate(json['toUtc']),
      outputPdfUri: json['outputPdfUri'] as String?,
      outputPdfBlobName: json['outputPdfBlobName'] as String?,
      outputFileName: json['outputFileName'] as String?,
      startedAtUtc: parseNullableDate(json['startedAtUtc'])?.toUtc(),
      completedAtUtc: parseNullableDate(json['completedAtUtc'])?.toUtc(),
      failureReason: json['failureReason'] as String?,
    );
  }

  bool get isPdfAvailable {
    final hasUri = outputPdfUri != null && outputPdfUri!.isNotEmpty;
    final hasBlob = outputPdfBlobName != null && outputPdfBlobName!.isNotEmpty;
    return hasUri || hasBlob;
  }
}

/// Matches:
/// { "runs": { items, totalCount, page, pageSize } }
class ListReportRunsResponseModel {
  final PagedResponseModel<ReportRunModel> runs;

  const ListReportRunsResponseModel({required this.runs});

  factory ListReportRunsResponseModel.fromJson(Map<String, dynamic> json) {
    final runsJson = (json['runs'] as Map?)?.cast<String, dynamic>() ??
        const <String, dynamic>{};

    return ListReportRunsResponseModel(
      runs: PagedResponseModel<ReportRunModel>.fromJson(
        runsJson,
        (e) => ReportRunModel.fromJson(e),
      ),
    );
  }
}

/// Local paged response wrapper (this endpoint's contract differs from shared PagedResponse).
class PagedResponseModel<T> {
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;

  const PagedResponseModel({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
  });

  factory PagedResponseModel.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic> itemJson) fromJsonT,
  ) {
    return PagedResponseModel<T>(
      items: (json['items'] as List<dynamic>? ?? const [])
          .whereType<Map>()
          .map((e) => fromJsonT(e.cast<String, dynamic>()))
          .toList(growable: false),
      totalCount: (json['totalCount'] as num?)?.toInt() ?? 0,
      page: (json['page'] as num?)?.toInt() ?? 1,
      pageSize: (json['pageSize'] as num?)?.toInt() ?? 20,
    );
  }
}
