using NutriTrack.Contracts.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Api.Common.Mappings
{
    public record UpdateMealMapping(Guid MealId, UserId UserId, UpdateMealRequest Request);
}
