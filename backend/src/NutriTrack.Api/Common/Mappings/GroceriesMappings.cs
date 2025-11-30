using Mapster;
using NutriTrack.Application.Groceries.Commands.CreateGrocery;
using NutriTrack.Application.Groceries.Commands.UpdateGrocery;
using NutriTrack.Application.Groceries.Queries.GetGroceryById;
using NutriTrack.Application.Groceries.Queries.ListGroceries;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Contracts.Groceries;
using NutriTrack.Domain.Groceries;
using NutriTrack.Application.Common.Models;

namespace NutriTrack.Api.Common.Mappings;

public class GroceriesMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateGroceryRequest, CreateGroceryCommand>();

        config.NewConfig<(Guid Id, UpdateGroceryRequest Request), UpdateGroceryCommand>()
            .Map(dest => dest.Id, src => new GroceryId(src.Id))
            .Map(dest => dest, src => src.Request);

        config.NewConfig<GetGroceryByIdRequest, GetGroceryByIdQuery>()
            .Map(dest => dest.Id, src => new NutriTrack.Domain.Groceries.GroceryId(src.Id));


        config.NewConfig<ListGroceriesRequest, ListGroceriesQuery>();

        config.NewConfig<GroceryResult, GroceryResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Category, src => src.Category.ToString())
            .Map(dest => dest.Barcode, src => src.Barcode)
            .Map(dest => dest.ProteinGramsPer100g, src => src.MacrosPer100.ProteinGramsPer100g)
            .Map(dest => dest.CarbsGramsPer100g, src => src.MacrosPer100.CarbsGramsPer100g)
            .Map(dest => dest.FatGramsPer100g, src => src.MacrosPer100.FatGramsPer100g)
            .Map(dest => dest.CaloriesPer100, src => src.CaloriesPer100)
            .Map(dest => dest.UnitOfMeasure, src => src.UnitOfMeasure.ToString())
            .Map(dest => dest.ImageUrl, src => src.ImageUrl)
            .Map(dest => dest.IsApproved, src => src.IsApproved)
            .Map(dest => dest.IsDeleted, src => src.IsDeleted);

        config.NewConfig<PagedResult<GroceryResult>, PagedResult<GroceryResponse>>()
            .Map(dest => dest.Items, src => src.Items);
    }
}