using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Contracts.WeightHistoryEntries;

public record WeightHistoryEntryResponse(
    Guid Id,
    Guid UserId,
    DateOnly Date,
    decimal WeightKg);