using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Reports.Services;

namespace NutriTrack.Api.Reports;

public sealed class ReportRunProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReportRunProcessor> _logger;

    public ReportRunProcessor(IServiceScopeFactory scopeFactory, ILogger<ReportRunProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IReportRunRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
                var pdfGenerator = scope.ServiceProvider.GetRequiredService<IReportPdfGenerator>();
                var outputStorage = scope.ServiceProvider.GetRequiredService<IReportOutputStorage>();

                var queued = await repo.ListQueuedAsync(take: 10, cancellationToken: stoppingToken);
                foreach (var run in queued)
                {
                    try
                    {
                        run.MarkRunning(clock.UtcNow);
                        await unitOfWork.SaveChangesAsync(stoppingToken);

                        var (fileName, pdfBytes) = await pdfGenerator.GenerateAsync(run, stoppingToken);
                        var (blobName, readUri) = await outputStorage.StorePdfAsync(pdfBytes, fileName, stoppingToken);

                        run.MarkReady(clock.UtcNow, blobName, fileName);
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to generate report run {ReportRunId}", run.Id.Value);

                        try
                        {
                            run.MarkFailed(clock.UtcNow, ex.Message);
                            await unitOfWork.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception markFailedEx)
                        {
                            _logger.LogError(markFailedEx, "Failed to mark report run {ReportRunId} as failed", run.Id.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing queued report runs");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
