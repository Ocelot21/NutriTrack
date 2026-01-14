import 'package:dio/dio.dart';
import 'package:nutritrack_shared/core/api_client.dart';
import 'package:nutritrack_shared/core/api_exception.dart';
import 'package:nutritrack_shared/core/token_store.dart';

import 'report_models.dart';

class ReportsException implements Exception {
  final String message;
  ReportsException(this.message);

  @override
  String toString() => 'ReportsException: $message';
}

class ReportPdfNotAvailableException extends ReportsException {
  ReportPdfNotAvailableException(super.message);
}

class ReportsRepo {
  final ApiClient _api;
  final TokenStore _tokenStore;

  ReportsRepo(this._api, this._tokenStore);

  /// GET /api/admin/reports/runs?page=1&pageSize=20
  Future<PagedResponseModel<ReportRunModel>> listRuns({
    required int page,
    required int pageSize,
  }) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/admin/reports/runs',
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
        },
      );

      final data = response.data ?? const <String, dynamic>{};
      final parsed = ListReportRunsResponseModel.fromJson(data);
      return parsed.runs;
    } on ApiException catch (e) {
      throw ReportsException(e.message);
    } catch (e) {
      throw ReportsException('Unexpected error: $e');
    }
  }

  /// GET /api/admin/reports/runs/{id}
  Future<ReportRunModel> getRun({required String id}) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.get<Map<String, dynamic>>(
        '/admin/reports/runs/$id',
      );

      final data = response.data ?? const <String, dynamic>{};
      return ReportRunModel.fromJson(data);
    } on ApiException catch (e) {
      throw ReportsException(e.message);
    } catch (e) {
      throw ReportsException('Unexpected error: $e');
    }
  }

  /// POST /api/admin/reports/runs
  Future<ReportRunModel> createRun(CreateReportRunRequestModel request) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      final response = await _api.post<Map<String, dynamic>>(
        '/admin/reports/runs',
        data: request.toJson(),
      );

      final data = response.data ?? const <String, dynamic>{};
      return ReportRunModel.fromJson(data);
    } on ApiException catch (e) {
      throw ReportsException(e.message);
    } catch (e) {
      throw ReportsException('Unexpected error: $e');
    }
  }

  /// GET /api/admin/reports/runs/{id}/pdf
  ///
  /// API redirects to a signed URL (or 404 if not available).
  /// Returns a URL to open in the browser.
  Future<Uri> getDownloadPdfUrl({required String id}) async {
    try {
      final token = await _tokenStore.read();
      _api.setAuthToken(token);

      // We need to observe the redirect location, so use the raw Dio instance.
      final dio = _api.raw;

      final resp = await dio.get<String>(
        '/admin/reports/runs/$id/pdf',
        options: Options(
          responseType: ResponseType.plain,
          followRedirects: false,
          validateStatus: (code) => code != null && code >= 200 && code < 400,
        ),
      );

      if (resp.statusCode == 200) {
        // Some servers return the final URL directly; attempt to parse.
        final s = (resp.data ?? '').trim();
        final uri = Uri.tryParse(s);
        if (uri != null && uri.hasScheme) return uri;
      }

      if (resp.statusCode == 302 ||
          resp.statusCode == 301 ||
          resp.statusCode == 303) {
        final location = resp.headers.value('location');
        final uri = location == null ? null : Uri.tryParse(location);
        if (uri != null && uri.hasScheme) return uri;
        throw ReportsException(
          'Report PDF redirect was missing a valid Location header.',
        );
      }

      // Some servers reply 204 when not ready; treat like not available.
      if (resp.statusCode == 404 || resp.statusCode == 204) {
        throw ReportPdfNotAvailableException('PDF not available yet.');
      }

      throw ReportsException('Unexpected PDF response: ${resp.statusCode}');
    } on DioException catch (e) {
      final status = e.response?.statusCode;
      if (status == 404) {
        throw ReportPdfNotAvailableException('PDF not available yet.');
      }
      throw ReportsException('Failed to download PDF: ${e.message}');
    } on ApiException catch (e) {
      throw ReportsException(e.message);
    } catch (e) {
      throw ReportsException('Unexpected error: $e');
    }
  }
}
