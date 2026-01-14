using NutriTrack.Application.Reports.Services;
using NutriTrack.Domain.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NutriTrack.Infrastructure.Reports.Pdf;

public sealed class QuestPdfReportPdfGenerator : IReportPdfGenerator
{
    private readonly IReportDataService _data;

    public QuestPdfReportPdfGenerator(IReportDataService data)
    {
        _data = data;
    }

    static QuestPdfReportPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<(string FileName, byte[] PdfBytes)> GenerateAsync(
        ReportRun reportRun,
        CancellationToken cancellationToken = default)
    {
        // Keep it intentionally simple: single-page summary, with optional 2nd page for details later.
        var title = reportRun.Type.ToString();
        var fileName = $"{title}-{reportRun.FromUtc:yyyyMMdd}-{reportRun.ToUtc:yyyyMMdd}.pdf";

        var weekly = reportRun.Type == ReportType.WeeklyOverview
            ? await _data.GetWeeklyOverviewAsync(reportRun, cancellationToken)
            : null;

        var userActivity = reportRun.Type == ReportType.UserActivity
            ? await _data.GetUserActivityAsync(reportRun, cancellationToken)
            : null;

        var adminAudit = reportRun.Type == ReportType.AdminAudit
            ? await _data.GetAdminAuditAsync(reportRun, cancellationToken)
            : null;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"{title} Report")
                    .SemiBold()
                    .FontSize(18);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text($"Report Id: {reportRun.Id.Value}");
                        r.ConstantItem(220).AlignRight().Text($"Range (UTC): {reportRun.FromUtc:yyyy-MM-dd} ? {reportRun.ToUtc:yyyy-MM-dd}");
                    });

                    col.Item().Text($"Requested At (UTC): {reportRun.RequestedAtUtc:u}");

                    if (weekly is not null)
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().Text("Summary").SemiBold();

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(1);
                            });

                            void Row(string label, string value)
                            {
                                t.Cell().Element(CellStyle).Text(label);
                                t.Cell().Element(CellStyle).AlignRight().Text(value);
                            }

                            Row("Meals logged", weekly.MealsCount.ToString());
                            Row("Calories consumed", weekly.TotalCaloriesConsumed.ToString());
                            Row("Protein (g)", weekly.TotalProteinGrams.ToString("0.##"));
                            Row("Carbs (g)", weekly.TotalCarbsGrams.ToString("0.##"));
                            Row("Fat (g)", weekly.TotalFatGrams.ToString("0.##"));
                            Row("Exercise sessions", weekly.ExerciseCount.ToString());
                            Row("Minutes exercised", weekly.TotalMinutesExercised.ToString("0.##"));
                            Row("Calories burned", weekly.TotalCaloriesBurned.ToString("0.##"));

                            static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                        });
                    }
                    else if (userActivity is not null)
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().Text("User activity summary").SemiBold();

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(1);
                            });

                            void Row(string label, string value)
                            {
                                t.Cell().Element(CellStyle).Text(label);
                                t.Cell().Element(CellStyle).AlignRight().Text(value);
                            }

                            Row("Meals logged", userActivity.MealsCount.ToString());
                            Row("Calories consumed", userActivity.TotalCaloriesConsumed.ToString());
                            Row("Protein (g)", userActivity.TotalProteinGrams.ToString("0.##"));
                            Row("Carbs (g)", userActivity.TotalCarbsGrams.ToString("0.##"));
                            Row("Fat (g)", userActivity.TotalFatGrams.ToString("0.##"));
                            Row("Exercise sessions", userActivity.ExerciseCount.ToString());
                            Row("Minutes exercised", userActivity.TotalMinutesExercised.ToString("0.##"));
                            Row("Calories burned", userActivity.TotalCaloriesBurned.ToString("0.##"));

                            static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                        });
                    }
                    else if (adminAudit is not null)
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().Text("Admin audit (derived)").SemiBold();
                        col.Item().Text("Counts are derived from Created/Modified metadata on core tables (not a dedicated audit log).")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        var total = adminAudit.Entities.Sum(x => x.CreatedCount + x.ModifiedCount);
                        if (total == 0)
                        {
                            col.Item().PaddingTop(5).Text("No create/modify activity was found in the selected time range.")
                                .FontColor(Colors.Grey.Darken1);
                            return;
                        }

                        col.Item().PaddingTop(5).Text("Activity by entity").SemiBold();
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(1);
                                c.RelativeColumn(1);
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("Entity").SemiBold();
                                h.Cell().AlignRight().Text("Created").SemiBold();
                                h.Cell().AlignRight().Text("Modified").SemiBold();
                            });

                            foreach (var row in adminAudit.Entities)
                            {
                                t.Cell().Element(CellStyle).Text(row.Entity);
                                t.Cell().Element(CellStyle).AlignRight().Text(row.CreatedCount.ToString());
                                t.Cell().Element(CellStyle).AlignRight().Text(row.ModifiedCount.ToString());
                            }

                            static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                        });

                        if (adminAudit.CreatedByUser.Count > 0)
                        {
                            col.Item().PaddingTop(10).Text("Creates (top)").SemiBold();
                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(3);
                                    c.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Text("UserId").SemiBold();
                                    h.Cell().AlignRight().Text("Count").SemiBold();
                                });

                                foreach (var row in adminAudit.CreatedByUser.Take(15))
                                {
                                    t.Cell().Element(CellStyle).Text(row.UserId?.Value.ToString() ?? "(system)");
                                    t.Cell().Element(CellStyle).AlignRight().Text(row.Count.ToString());
                                }

                                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                            });
                        }

                        if (adminAudit.ModifiedByUser.Count > 0)
                        {
                            col.Item().PaddingTop(10).Text("Modifications (top)").SemiBold();
                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(3);
                                    c.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Text("UserId").SemiBold();
                                    h.Cell().AlignRight().Text("Count").SemiBold();
                                });

                                foreach (var row in adminAudit.ModifiedByUser.Take(15))
                                {
                                    t.Cell().Element(CellStyle).Text(row.UserId?.Value.ToString() ?? "(system)");
                                    t.Cell().Element(CellStyle).AlignRight().Text(row.Count.ToString());
                                }

                                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6);
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(reportRun.ParametersJson))
                        {
                            col.Item().Text("Parameters:").SemiBold();
                            col.Item().Background(Colors.Grey.Lighten4).Padding(10).Text(reportRun.ParametersJson).FontSize(9);
                        }

                        col.Item().LineHorizontal(1);
                        col.Item().Text("Summary").SemiBold();
                        col.Item().Text("This report type is not implemented yet.");
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken1));
                    text.Span("Generated by NutriTrack • ");
                    text.Span(DateTime.UtcNow.ToString("u"));
                });
            });
        }).GeneratePdf();

        return (fileName, pdf);
    }
}
