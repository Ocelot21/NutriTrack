using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Common.Mappings;

public static class GroceryMappings
{
    public static GroceryResult ToGroceryResult(this Grocery grocery)
    {
        return new GroceryResult(
            grocery.Id,
            grocery.Name,
            grocery.Category,
            grocery.Barcode,
            grocery.MacrosPer100,
            grocery.CaloriesPer100,
            grocery.UnitOfMeasure,
            grocery.ImageUrl,
            grocery.IsApproved,
            grocery.IsDeleted
        );
    }

    public static PagedResult<GroceryResult> ToGroceryPagedResult(this PagedResult<Grocery> pagedGroceries)
    {
        var groceryResults = pagedGroceries.Items.Select(grocery => grocery.ToGroceryResult()).ToList();
        return new PagedResult<GroceryResult>(
            groceryResults,
            pagedGroceries.TotalCount,
            pagedGroceries.Page,
            pagedGroceries.PageSize
        );
    }
}
