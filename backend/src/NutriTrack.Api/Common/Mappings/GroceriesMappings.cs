using Mapster;
using NutriTrack.Application.Groceries.Commands.CreateGrocery;
using NutriTrack.Application.Groceries.Commands.UpdateGrocery;
using NutriTrack.Application.Groceries.Queries.GetGroceryById;
using NutriTrack.Application.Groceries.Queries.ListGroceries;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Contracts.Groceries;
using NutriTrack.Domain.Groceries;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;
using Microsoft.AspNetCore.Http;
using NutriTrack.Application.Groceries.Commands.CreateGrocerySuggestion;

namespace NutriTrack.Api.Common.Mappings;

public class GroceriesMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CreateGroceryRequest Request, IFormFile? Image), CreateGroceryCommand>()
            .ConstructUsing(src => new CreateGroceryCommand(
                src.Request.Name,
                Enum.Parse<GroceryCategory>(src.Request.Category, true),
                src.Request.ProteinPer100,
                src.Request.CarbsPer100,
                src.Request.FatPer100,
                src.Request.CaloriesPer100,
                Enum.Parse<UnitOfMeasure>(src.Request.UnitOfMeasure, true),
                src.Request.GramsPerPiece,
                src.Request.Barcode,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null));

        config.NewConfig<(Guid Id, UpdateGroceryRequest Request, IFormFile? Image), UpdateGroceryCommand>()
            .ConstructUsing(src => new UpdateGroceryCommand(
                new GroceryId(src.Id),
                src.Request.Name,
                string.IsNullOrWhiteSpace(src.Request.Category) ? null : Enum.Parse<GroceryCategory>(src.Request.Category!, true),
                src.Request.ProteinPer100,
                src.Request.CarbsPer100,
                src.Request.FatPer100,
                src.Request.CaloriesPer100,
                string.IsNullOrWhiteSpace(src.Request.UnitOfMeasure) ? null : Enum.Parse<UnitOfMeasure>(src.Request.UnitOfMeasure!, true),
                string.IsNullOrWhiteSpace(src.Request.GramsPerPiece) ? (decimal?)null : decimal.Parse(src.Request.GramsPerPiece!),
                src.Request.Barcode,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null));

        config.NewConfig<GetGroceryByIdRequest, GetGroceryByIdQuery>()
            .Map(dest => dest.Id, src => new GroceryId(src.Id));

        config.NewConfig<GroceryResult, GroceryResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Category, src => src.Category.ToString())
            .Map(dest => dest.Barcode, src => src.Barcode)
            .Map(dest => dest.ProteinGramsPer100, src => src.MacrosPer100.ProteinGramsPer100)
            .Map(dest => dest.CarbsGramsPer100, src => src.MacrosPer100.CarbsGramsPer100)
            .Map(dest => dest.FatGramsPer100, src => src.MacrosPer100.FatGramsPer100)
            .Map(dest => dest.CaloriesPer100, src => src.CaloriesPer100)
            .Map(dest => dest.UnitOfMeasure, src => src.UnitOfMeasure.ToString())
            .Map(dest => dest.GramsPerPiece, src => src.GramsPerPiece)
            .Map(dest => dest.ImageUrl, src => src.ImageUrl)
            .Map(dest => dest.IsApproved, src => src.IsApproved)
            .Map(dest => dest.IsDeleted, src => src.IsDeleted);

        config.NewConfig<PagedResult<GroceryResult>, PagedResult<GroceryResponse>>()
            .Map(dest => dest.Items, src => src.Items);

        config.NewConfig<(UserId UserId, ListGroceriesRequest Request), ListGroceriesQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.Request)
            .Map(dest => dest.Filters, src => src.Request);

        config.NewConfig<(CreateGroceryRequest Request, IFormFile? Image), CreateGrocerySuggestionCommand>()
            .ConstructUsing(src => new CreateGrocerySuggestionCommand(
                src.Request.Name,
                Enum.Parse<GroceryCategory>(src.Request.Category, true),
                src.Request.ProteinPer100,
                src.Request.CarbsPer100,
                src.Request.FatPer100,
                src.Request.CaloriesPer100,
                Enum.Parse<UnitOfMeasure>(src.Request.UnitOfMeasure, true),
                src.Request.GramsPerPiece,
                src.Request.Barcode,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null));
    }
}