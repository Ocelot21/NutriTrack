using Mapster;
using NutriTrack.Application.Meals.Commands.CreateMeal;
using NutriTrack.Application.Meals.Commands.DeleteMeal;
using NutriTrack.Application.Meals.Commands.AddMealItem;
using NutriTrack.Application.Meals.Commands.UpdateMealItem;
using NutriTrack.Application.Meals.Commands.RemoveMealItem;
using NutriTrack.Domain.Meals;
using NutriTrack.Contracts.Meals;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Meals.Common;

namespace NutriTrack.Api.Common.Mappings;

public class MealsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateMealRequest, CreateMealCommand>();

        config.NewConfig<DeleteMealRequest, DeleteMealCommand>()
            .Map(dest => dest.Id, src => new NutriTrack.Domain.Meals.MealId(src.MealId));

        config.NewConfig<AddMealItemRequest, AddMealItemCommand>()
            .Map(dest => dest.MealId, src => new NutriTrack.Domain.Meals.MealId(src.MealId))
            .Map(dest => dest.GroceryId, src => new NutriTrack.Domain.Groceries.GroceryId(src.GroceryId));

        config.NewConfig<DeleteMealItemRequest, RemoveMealItemCommand>()
            .Map(dest => dest.MealId, src => new NutriTrack.Domain.Meals.MealId(src.MealId))
            .Map(dest => dest.MealItemId, src => new NutriTrack.Domain.Meals.MealItemId(src.MealItemId));

        config.NewConfig<MealResult, MealResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.OccurredAtUtc, src => src.OccurredAtUtc)
            .Map(dest => dest.OccurredAtLocal, src => src.OccurredAtLocal)
            .Map(dest => dest.LocalDate, src => src.LocalDate)
            .Map(dest => dest.TotalCalories, src => src.TotalCalories)
            .Map(dest => dest.TotalProtein, src => src.TotalProtein)
            .Map(dest => dest.TotalCarbohydrates, src => src.TotalCarbohydrates)
            .Map(dest => dest.TotalFats, src => src.TotalFats)
            .Map(dest => dest.Items, src => src.Items.Select(i => new MealItemResponse(
                i.Id.Value,
                i.GroceryId.Value,
                i.Snapshot.GroceryName,
                i.Snapshot.CaloriesPer100,
                i.Snapshot.MacrosPer100.ProteinGramsPer100g,
                i.Snapshot.MacrosPer100.CarbsGramsPer100g,
                i.Snapshot.MacrosPer100.FatGramsPer100g,
                i.Snapshot.UnitOfMeasure.ToString(),
                i.Quantity)).ToList());

        config.NewConfig<PagedResult<Meal>, NutriTrack.Contracts.Common.PagedResponse<MealResponse>>()
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.TotalCount, src => src.TotalCount)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);
    }
}
