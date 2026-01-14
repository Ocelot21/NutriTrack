using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Reports.Common;

public sealed record AdminAuditReportData(
    IReadOnlyList<EntityAuditRow> Entities,
    IReadOnlyList<AuditCountRow> CreatedByUser,
    IReadOnlyList<AuditCountRow> ModifiedByUser);

public sealed record EntityAuditRow(
    string Entity,
    int CreatedCount,
    int ModifiedCount);

public sealed record AuditCountRow(
    UserId? UserId,
    int Count);
