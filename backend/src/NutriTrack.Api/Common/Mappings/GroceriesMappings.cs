using Mapster;
using NutriTrack.Application.Groceries.Commands.CreateGrocery;
using NutriTrack.Application.Groceries.Commands.UpdateGrocery;
using NutriTrack.Application.Groceries.Queries.GetGroceryById;
using NutriTrack.Application.Groceries.Queries.ListGroceries;
using NutriTrack.Application.Groceries.Common;
//using NutriTrack.Contracts.Groceries;

namespace NutriTrack.Api.Common.Mappings;

public class GroceriesMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.NewConfig<CreateGroceryRequest, CreateGroceryCommand>();
        //config.NewConfig<UpdateGroceryRequest, UpdateGroceryCommand>();
        //config.NewConfig<GetGroceryByIdRequest, GetGroceryByIdQuery>();
        //config.NewConfig<ListGroceriesRequest, ListGroceriesQuery>();
        //config.NewConfig<GroceryResult, GroceryResponse>();
    }
}
