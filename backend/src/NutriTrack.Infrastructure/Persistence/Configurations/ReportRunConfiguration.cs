using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Reports;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class ReportRunConfiguration : IEntityTypeConfiguration<ReportRun>
{
    public void Configure(EntityTypeBuilder<ReportRun> builder)
    {
        builder.ToTable("ReportRuns");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasReportRunIdConversion();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RequestedBy)
            .HasUserIdConversion()
            .IsRequired();

        builder.Property(x => x.RequestedAtUtc)
            .IsRequired();

        builder.Property(x => x.FromUtc)
            .IsRequired();

        builder.Property(x => x.ToUtc)
            .IsRequired();

        builder.Property(x => x.ParametersJson)
            .HasMaxLength(8000);

        builder.Property(x => x.OutputPdfUri)
            .HasMaxLength(1024);

        builder.Property(x => x.OutputPdfBlobName)
            .HasMaxLength(256);

        builder.Property(x => x.OutputFileName)
            .HasMaxLength(256);

        builder.Property(x => x.StartedAtUtc);

        builder.Property(x => x.CompletedAtUtc);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.RequestedBy);
        builder.HasIndex(x => x.RequestedAtUtc);

        builder.ConfigureAuditable();
    }
}
