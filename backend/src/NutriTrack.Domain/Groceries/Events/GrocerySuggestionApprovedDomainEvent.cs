using NutriTrack.Domain.Common.Events;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Groceries.Events;

public sealed record GrocerySuggestionApprovedDomainEvent(
    GroceryId GroceryId,
    UserId SuggestedByUserId) : IDomainEvent;
