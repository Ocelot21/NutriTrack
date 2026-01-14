using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Infrastructure.Persistence.Seed.Groceries;

public sealed class GrocerySeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public GrocerySeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 70;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var groceries = new (string Name, GroceryCategory Category, decimal Protein, decimal Carbs, decimal Fat, int CaloriesPer100, UnitOfMeasure Uom, decimal? GramsPerPiece)[]
        {
            ("Apple", GroceryCategory.Fruit, 0.30m, 13.80m, 0.20m, 52, UnitOfMeasure.Gram, null),
            ("Banana", GroceryCategory.Fruit, 1.10m, 22.80m, 0.30m, 89, UnitOfMeasure.Gram, null),
            ("Orange", GroceryCategory.Fruit, 0.90m, 11.80m, 0.10m, 47, UnitOfMeasure.Gram, null),
            ("Strawberries", GroceryCategory.Fruit, 0.70m, 7.70m, 0.30m, 32, UnitOfMeasure.Gram, null),
            ("Blueberries", GroceryCategory.Fruit, 0.70m, 14.50m, 0.30m, 57, UnitOfMeasure.Gram, null),
            ("Grapes", GroceryCategory.Fruit, 0.60m, 18.10m, 0.20m, 69, UnitOfMeasure.Gram, null),
            ("Pineapple", GroceryCategory.Fruit, 0.50m, 13.10m, 0.10m, 50, UnitOfMeasure.Gram, null),
            ("Mango", GroceryCategory.Fruit, 0.80m, 15.00m, 0.40m, 60, UnitOfMeasure.Gram, null),
            ("Watermelon", GroceryCategory.Fruit, 0.60m, 7.60m, 0.20m, 30, UnitOfMeasure.Gram, null),
            ("Avocado", GroceryCategory.Fruit, 2.00m, 8.50m, 14.70m, 160, UnitOfMeasure.Gram, null),

            ("Carrot", GroceryCategory.Vegetable, 0.90m, 9.60m, 0.20m, 41, UnitOfMeasure.Gram, null),
            ("Broccoli", GroceryCategory.Vegetable, 2.80m, 6.60m, 0.40m, 34, UnitOfMeasure.Gram, null),
            ("Spinach", GroceryCategory.Vegetable, 2.90m, 3.60m, 0.40m, 23, UnitOfMeasure.Gram, null),
            ("Tomato", GroceryCategory.Vegetable, 0.90m, 3.90m, 0.20m, 18, UnitOfMeasure.Gram, null),
            ("Cucumber", GroceryCategory.Vegetable, 0.70m, 3.60m, 0.10m, 16, UnitOfMeasure.Gram, null),
            ("Bell Pepper", GroceryCategory.Vegetable, 1.00m, 6.00m, 0.30m, 31, UnitOfMeasure.Gram, null),
            ("Onion", GroceryCategory.Vegetable, 1.10m, 9.30m, 0.10m, 40, UnitOfMeasure.Gram, null),
            ("Garlic", GroceryCategory.Vegetable, 6.40m, 33.10m, 0.50m, 149, UnitOfMeasure.Gram, null),
            ("Potato", GroceryCategory.Vegetable, 2.00m, 17.50m, 0.10m, 77, UnitOfMeasure.Gram, null),
            ("Sweet Potato", GroceryCategory.Vegetable, 1.60m, 20.10m, 0.10m, 86, UnitOfMeasure.Gram, null),
            ("Lettuce", GroceryCategory.Vegetable, 1.40m, 2.90m, 0.20m, 15, UnitOfMeasure.Gram, null),
            ("Mushrooms", GroceryCategory.Vegetable, 3.10m, 3.30m, 0.30m, 22, UnitOfMeasure.Gram, null),
            ("Zucchini", GroceryCategory.Vegetable, 1.20m, 3.10m, 0.30m, 17, UnitOfMeasure.Gram, null),
            ("Cauliflower", GroceryCategory.Vegetable, 1.90m, 5.00m, 0.30m, 25, UnitOfMeasure.Gram, null),
            ("Green Peas", GroceryCategory.Vegetable, 5.40m, 14.50m, 0.40m, 81, UnitOfMeasure.Gram, null),

            ("White Rice (cooked)", GroceryCategory.Grain, 2.70m, 28.20m, 0.30m, 130, UnitOfMeasure.Gram, null),
            ("Brown Rice (cooked)", GroceryCategory.Grain, 2.60m, 23.00m, 0.90m, 111, UnitOfMeasure.Gram, null),
            ("Oats", GroceryCategory.Grain, 16.90m, 66.30m, 6.90m, 389, UnitOfMeasure.Gram, null),
            ("Pasta (cooked)", GroceryCategory.Grain, 5.80m, 30.90m, 0.90m, 158, UnitOfMeasure.Gram, null),
            ("Whole Wheat Bread", GroceryCategory.Grain, 13.00m, 41.00m, 4.20m, 247, UnitOfMeasure.Gram, null),
            ("Corn Tortilla", GroceryCategory.Grain, 5.70m, 45.80m, 3.00m, 218, UnitOfMeasure.Gram, null),
            ("Quinoa (cooked)", GroceryCategory.Grain, 4.40m, 21.30m, 1.90m, 120, UnitOfMeasure.Gram, null),
            ("Couscous (cooked)", GroceryCategory.Grain, 3.80m, 23.20m, 0.20m, 112, UnitOfMeasure.Gram, null),

            ("Chicken Breast (cooked)", GroceryCategory.Protein, 31.00m, 0.00m, 3.60m, 165, UnitOfMeasure.Gram, null),
            ("Turkey Breast (cooked)", GroceryCategory.Protein, 29.00m, 0.00m, 1.00m, 135, UnitOfMeasure.Gram, null),
            ("Salmon", GroceryCategory.Protein, 20.40m, 0.00m, 13.40m, 208, UnitOfMeasure.Gram, null),
            ("Tuna (canned in water)", GroceryCategory.Protein, 23.60m, 0.00m, 0.80m, 116, UnitOfMeasure.Gram, null),
            ("Egg", GroceryCategory.Protein, 12.60m, 1.10m, 10.00m, 143, UnitOfMeasure.Piece, 50m),
            ("Tofu (firm)", GroceryCategory.Protein, 15.70m, 1.90m, 8.70m, 144, UnitOfMeasure.Gram, null),
            ("Lentils (cooked)", GroceryCategory.Protein, 9.00m, 20.10m, 0.40m, 116, UnitOfMeasure.Gram, null),
            ("Chickpeas (cooked)", GroceryCategory.Protein, 8.90m, 27.40m, 2.60m, 164, UnitOfMeasure.Gram, null),
            ("Black Beans (cooked)", GroceryCategory.Protein, 8.90m, 23.70m, 0.50m, 132, UnitOfMeasure.Gram, null),
            ("Peanut Butter", GroceryCategory.Protein, 25.00m, 20.00m, 50.00m, 588, UnitOfMeasure.Gram, null),

            ("Milk 2%", GroceryCategory.Dairy, 3.40m, 5.00m, 2.00m, 50, UnitOfMeasure.Milliliter, null),
            ("Whole Milk", GroceryCategory.Dairy, 3.20m, 4.80m, 3.30m, 61, UnitOfMeasure.Milliliter, null),
            ("Greek Yogurt (plain)", GroceryCategory.Dairy, 10.00m, 3.60m, 0.40m, 59, UnitOfMeasure.Gram, null),
            ("Cheddar Cheese", GroceryCategory.Dairy, 25.00m, 1.30m, 33.00m, 403, UnitOfMeasure.Gram, null),
            ("Cottage Cheese", GroceryCategory.Dairy, 11.10m, 3.40m, 4.30m, 98, UnitOfMeasure.Gram, null),

            ("Olive Oil", GroceryCategory.Other, 0.00m, 0.00m, 100.00m, 884, UnitOfMeasure.Gram, null),
            ("Butter", GroceryCategory.Other, 0.90m, 0.10m, 81.00m, 717, UnitOfMeasure.Gram, null),
            ("Honey", GroceryCategory.Other, 0.30m, 82.40m, 0.00m, 304, UnitOfMeasure.Gram, null),

            ("Almonds", GroceryCategory.Snack, 21.20m, 21.60m, 49.90m, 579, UnitOfMeasure.Gram, null),
            ("Walnuts", GroceryCategory.Snack, 15.20m, 13.70m, 65.20m, 654, UnitOfMeasure.Gram, null),
            ("Peanuts", GroceryCategory.Snack, 25.80m, 16.10m, 49.20m, 567, UnitOfMeasure.Gram, null),
            ("Dark Chocolate 70%", GroceryCategory.Snack, 7.80m, 45.90m, 42.60m, 598, UnitOfMeasure.Gram, null),
            ("Potato Chips", GroceryCategory.Snack, 6.60m, 53.50m, 35.40m, 536, UnitOfMeasure.Gram, null),
            ("Popcorn (air-popped)", GroceryCategory.Snack, 12.90m, 77.80m, 4.30m, 387, UnitOfMeasure.Gram, null),

            ("Coca-Cola", GroceryCategory.Drink, 0.00m, 10.60m, 0.00m, 42, UnitOfMeasure.Milliliter, null),
            ("Orange Juice", GroceryCategory.Drink, 0.70m, 10.40m, 0.20m, 45, UnitOfMeasure.Milliliter, null),
            ("Apple Juice", GroceryCategory.Drink, 0.10m, 11.30m, 0.10m, 46, UnitOfMeasure.Milliliter, null),
            ("Coffee (black)", GroceryCategory.Drink, 0.10m, 0.00m, 0.00m, 1, UnitOfMeasure.Milliliter, null),
            ("Green Tea", GroceryCategory.Drink, 0.00m, 0.20m, 0.00m, 1, UnitOfMeasure.Milliliter, null),
            ("Sparkling Water", GroceryCategory.Drink, 0.00m, 0.00m, 0.00m, 0, UnitOfMeasure.Milliliter, null),

            ("Salt", GroceryCategory.Other, 0.00m, 0.00m, 0.00m, 0, UnitOfMeasure.Gram, null),
            ("Black Pepper", GroceryCategory.Other, 10.40m, 63.90m, 3.30m, 251, UnitOfMeasure.Gram, null),
            ("Ketchup", GroceryCategory.Other, 1.70m, 25.80m, 0.20m, 112, UnitOfMeasure.Gram, null),
            ("Mustard", GroceryCategory.Other, 4.40m, 5.80m, 4.40m, 66, UnitOfMeasure.Gram, null),

            ("Pear", GroceryCategory.Fruit, 0.40m, 15.20m, 0.10m, 57, UnitOfMeasure.Gram, null),
            ("Peach", GroceryCategory.Fruit, 0.90m, 9.50m, 0.30m, 39, UnitOfMeasure.Gram, null),
            ("Kiwi", GroceryCategory.Fruit, 1.10m, 14.70m, 0.50m, 61, UnitOfMeasure.Gram, null),
            ("Lemon", GroceryCategory.Fruit, 1.10m, 9.30m, 0.30m, 29, UnitOfMeasure.Gram, null),
            ("Cherries", GroceryCategory.Fruit, 1.10m, 16.00m, 0.20m, 63, UnitOfMeasure.Gram, null),

            ("Asparagus", GroceryCategory.Vegetable, 2.20m, 3.90m, 0.10m, 20, UnitOfMeasure.Gram, null),
            ("Cabbage", GroceryCategory.Vegetable, 1.30m, 5.80m, 0.10m, 25, UnitOfMeasure.Gram, null),
            ("Celery", GroceryCategory.Vegetable, 0.70m, 3.00m, 0.20m, 16, UnitOfMeasure.Gram, null),
            ("Kale", GroceryCategory.Vegetable, 4.30m, 8.80m, 0.90m, 49, UnitOfMeasure.Gram, null),
            ("Eggplant", GroceryCategory.Vegetable, 1.00m, 5.90m, 0.20m, 25, UnitOfMeasure.Gram, null),

            ("White Rice (dry)", GroceryCategory.Grain, 7.10m, 80.00m, 0.70m, 365, UnitOfMeasure.Gram, null),
            ("Brown Rice (dry)", GroceryCategory.Grain, 7.50m, 76.00m, 2.70m, 370, UnitOfMeasure.Gram, null),
            ("Flour (all-purpose)", GroceryCategory.Grain, 10.30m, 76.30m, 1.00m, 364, UnitOfMeasure.Gram, null),
            ("Corn Flakes", GroceryCategory.Grain, 7.00m, 84.00m, 0.40m, 357, UnitOfMeasure.Gram, null),
            ("Granola", GroceryCategory.Grain, 10.00m, 64.00m, 15.00m, 471, UnitOfMeasure.Gram, null),

            ("Beef Steak (lean)", GroceryCategory.Protein, 26.00m, 0.00m, 10.00m, 217, UnitOfMeasure.Gram, null),
            ("Pork Chop (lean)", GroceryCategory.Protein, 22.00m, 0.00m, 9.00m, 196, UnitOfMeasure.Gram, null),
            ("Shrimp", GroceryCategory.Protein, 24.00m, 0.20m, 0.30m, 99, UnitOfMeasure.Gram, null),
            ("Sardines (canned)", GroceryCategory.Protein, 25.00m, 0.00m, 11.00m, 208, UnitOfMeasure.Gram, null),
            ("Tempeh", GroceryCategory.Protein, 19.00m, 9.00m, 11.00m, 193, UnitOfMeasure.Gram, null),

            ("Mozzarella Cheese", GroceryCategory.Dairy, 28.00m, 3.10m, 17.00m, 280, UnitOfMeasure.Gram, null),
            ("Parmesan Cheese", GroceryCategory.Dairy, 35.80m, 3.20m, 25.80m, 431, UnitOfMeasure.Gram, null),
            ("Butter Milk", GroceryCategory.Dairy, 3.30m, 4.80m, 1.00m, 40, UnitOfMeasure.Milliliter, null),
            ("Skim Milk", GroceryCategory.Dairy, 3.40m, 5.00m, 0.10m, 34, UnitOfMeasure.Milliliter, null),
            ("Sour Cream", GroceryCategory.Dairy, 2.20m, 4.60m, 19.00m, 193, UnitOfMeasure.Gram, null),

            ("Cottage Cheese (low-fat)", GroceryCategory.Dairy, 12.40m, 3.00m, 1.50m, 72, UnitOfMeasure.Gram, null),
            ("Yogurt (vanilla)", GroceryCategory.Dairy, 3.50m, 17.00m, 3.00m, 105, UnitOfMeasure.Gram, null),

            ("Sunflower Seeds", GroceryCategory.Snack, 20.80m, 20.00m, 51.50m, 584, UnitOfMeasure.Gram, null),
            ("Pumpkin Seeds", GroceryCategory.Snack, 30.20m, 10.70m, 49.00m, 559, UnitOfMeasure.Gram, null),
            ("Granola Bar", GroceryCategory.Snack, 6.00m, 66.00m, 12.00m, 430, UnitOfMeasure.Gram, null),
            ("Rice Cakes", GroceryCategory.Snack, 7.30m, 81.00m, 2.80m, 387, UnitOfMeasure.Gram, null),
            ("Pretzels", GroceryCategory.Snack, 9.70m, 80.00m, 2.60m, 380, UnitOfMeasure.Gram, null),

            ("Ice Cream (vanilla)", GroceryCategory.Snack, 3.50m, 23.60m, 11.00m, 207, UnitOfMeasure.Gram, null),

            ("Mineral Water", GroceryCategory.Drink, 0.00m, 0.00m, 0.00m, 0, UnitOfMeasure.Milliliter, null),
            ("Milkshake (vanilla)", GroceryCategory.Drink, 3.50m, 19.00m, 4.00m, 120, UnitOfMeasure.Milliliter, null),
            ("Sports Drink", GroceryCategory.Drink, 0.00m, 6.00m, 0.00m, 24, UnitOfMeasure.Milliliter, null),
            ("Energy Drink", GroceryCategory.Drink, 0.00m, 11.00m, 0.00m, 44, UnitOfMeasure.Milliliter, null),

            ("Kefir", GroceryCategory.Dairy, 3.60m, 4.50m, 2.00m, 55, UnitOfMeasure.Milliliter, null),

            ("Protein Bar", GroceryCategory.Snack, 25.00m, 45.00m, 10.00m, 370, UnitOfMeasure.Gram, null),

            ("Caesar Dressing", GroceryCategory.Other, 2.00m, 7.00m, 45.00m, 470, UnitOfMeasure.Gram, null),
            ("Mayonnaise", GroceryCategory.Other, 1.00m, 0.60m, 75.00m, 680, UnitOfMeasure.Gram, null),
            ("Soy Sauce", GroceryCategory.Other, 8.00m, 5.00m, 0.10m, 53, UnitOfMeasure.Gram, null),
            ("Vinegar", GroceryCategory.Other, 0.00m, 0.00m, 0.00m, 18, UnitOfMeasure.Milliliter, null),
            ("Tomato Paste", GroceryCategory.Other, 4.80m, 18.90m, 0.50m, 82, UnitOfMeasure.Gram, null),

            ("Almond Milk (unsweetened)", GroceryCategory.Drink, 0.40m, 0.30m, 1.10m, 13, UnitOfMeasure.Milliliter, null),
            ("Oat Milk", GroceryCategory.Drink, 1.00m, 6.70m, 1.50m, 46, UnitOfMeasure.Milliliter, null),
            ("Coconut Water", GroceryCategory.Drink, 0.70m, 3.70m, 0.20m, 19, UnitOfMeasure.Milliliter, null),
            ("Lemonade", GroceryCategory.Drink, 0.00m, 10.00m, 0.00m, 40, UnitOfMeasure.Milliliter, null),

            ("Pita Bread", GroceryCategory.Grain, 9.00m, 56.00m, 1.20m, 275, UnitOfMeasure.Gram, null),
            ("Bagel", GroceryCategory.Grain, 10.40m, 53.00m, 1.50m, 250, UnitOfMeasure.Piece, 95m),

            ("Apple (medium)", GroceryCategory.Fruit, 0.30m, 13.80m, 0.20m, 52, UnitOfMeasure.Piece, 182m),
            ("Banana (medium)", GroceryCategory.Fruit, 1.10m, 22.80m, 0.30m, 89, UnitOfMeasure.Piece, 118m),
            ("Orange (medium)", GroceryCategory.Fruit, 0.90m, 11.80m, 0.10m, 47, UnitOfMeasure.Piece, 131m),

            ("Egg (large)", GroceryCategory.Protein, 12.60m, 1.10m, 10.00m, 143, UnitOfMeasure.Piece, 50m),

            ("Chicken Thigh (cooked)", GroceryCategory.Protein, 24.00m, 0.00m, 10.00m, 209, UnitOfMeasure.Gram, null),
            ("Ground Beef 10% fat", GroceryCategory.Protein, 20.00m, 0.00m, 10.00m, 176, UnitOfMeasure.Gram, null),

            ("Frozen Mixed Vegetables", GroceryCategory.Vegetable, 3.00m, 10.00m, 0.50m, 55, UnitOfMeasure.Gram, null),
            ("Frozen Berries", GroceryCategory.Fruit, 1.00m, 12.00m, 0.50m, 50, UnitOfMeasure.Gram, null),

            ("Hummus", GroceryCategory.Protein, 8.00m, 14.00m, 9.00m, 166, UnitOfMeasure.Gram, null),

            ("Canned Corn", GroceryCategory.Vegetable, 3.40m, 18.70m, 1.20m, 96, UnitOfMeasure.Gram, null),
            ("Canned Tomatoes", GroceryCategory.Vegetable, 1.10m, 4.00m, 0.20m, 18, UnitOfMeasure.Gram, null),

            ("Granulated Sugar", GroceryCategory.Other, 0.00m, 100.00m, 0.00m, 387, UnitOfMeasure.Gram, null),

            ("Cinnamon", GroceryCategory.Other, 4.00m, 81.00m, 1.20m, 247, UnitOfMeasure.Gram, null),
            ("Paprika", GroceryCategory.Other, 14.00m, 54.00m, 13.00m, 282, UnitOfMeasure.Gram, null),

            ("Baking Powder", GroceryCategory.Other, 0.00m, 28.00m, 0.00m, 53, UnitOfMeasure.Gram, null),

            ("Raisins", GroceryCategory.Snack, 3.10m, 79.20m, 0.50m, 299, UnitOfMeasure.Gram, null),

            ("Peanut (piece)", GroceryCategory.Snack, 25.80m, 16.10m, 49.20m, 567, UnitOfMeasure.Piece, 1.0m),
        };

        var existingNames = await _dbContext.Groceries
            .Select(g => g.Name)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);

        var toInsert = groceries
            .Where(g => !existingSet.Contains(g.Name))
            .Select(g => Grocery.Create(
                name: g.Name,
                category: g.Category,
                macros: new MacroNutrients(g.Protein, g.Carbs, g.Fat),
                caloriesPer100: g.CaloriesPer100,
                unitOfMeasure: g.Uom,
                gramsPerPiece: g.GramsPerPiece,
                barcode: null,
                imageUrl: null,
                isApproved: true))
            .ToArray();

        if (toInsert.Length > 0)
        {
            _dbContext.Groceries.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
