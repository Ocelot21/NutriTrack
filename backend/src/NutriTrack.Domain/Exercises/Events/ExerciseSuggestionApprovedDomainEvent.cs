using NutriTrack.Domain.Common.Events;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Exercises.Events;

public sealed record ExerciseSuggestionApprovedDomainEvent(
    ExerciseId ExerciseId,
    UserId SuggestedByUserId) : IDomainEvent;
