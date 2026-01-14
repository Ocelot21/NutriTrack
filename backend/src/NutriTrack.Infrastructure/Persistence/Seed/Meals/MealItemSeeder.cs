using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.Meals;

public sealed class MealItemSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public MealItemSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // Run after MealSeeder
    public int Order => 121;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = today.AddDays(-6);

        var groceries = await _dbContext.Groceries
            .Select(g => new { g.Id, g.Name })
            .ToListAsync(cancellationToken);

        var groceryIdByName = groceries.ToDictionary(g => g.Name, g => g.Id, StringComparer.OrdinalIgnoreCase);

        async Task<Grocery> GetGroceryAsync(string name)
        {
            if (!groceryIdByName.TryGetValue(name, out var id))
                throw new InvalidOperationException($"Grocery '{name}' not found, run GrocerySeeder first!");

            return await _dbContext.Groceries.SingleAsync(g => g.Id == id, cancellationToken);
        }

        // Get user nutrition goals for meal owners in the window.
        var userGoals = await _dbContext.Users
            .Select(u => new { u.Id, u.NutritionGoal })
            .ToListAsync(cancellationToken);

        var nutritionGoalByUserId = userGoals.ToDictionary(u => u.Id, u => u.NutritionGoal);

        // Load meal ids in the window (no tracking graph loaded up-front).
        var mealIds = await _dbContext.Meals
            .Where(m => m.LocalDate >= from && m.LocalDate <= today)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        foreach (var mealId in mealIds)
        {
            // Load meal as a tracked aggregate, but keep the graph small and explicit.
            var meal = await _dbContext.Meals.SingleAsync(m => m.Id == mealId, cancellationToken);

            await _dbContext.Entry(meal)
                .Collection(m => m.Items)
                .LoadAsync(cancellationToken);

            if (meal.Items.Count > 0)
            {
                _dbContext.ChangeTracker.Clear();
                continue;
            }

            if (!nutritionGoalByUserId.TryGetValue(meal.UserId, out var goal))
            {
                goal = NutritionGoal.MaintainWeight;
            }

            var items = GetItemsFor(meal, goal);

            if (items.Length == 0)
            {
                _dbContext.ChangeTracker.Clear();
                continue;
            }

            foreach (var (groceryName, qty) in items)
            {
                if (!groceryIdByName.ContainsKey(groceryName))
                {
                    continue;
                }

                var grocery = await GetGroceryAsync(groceryName);
                meal.AddItem(grocery, qty);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
        }

        await tx.CommitAsync(cancellationToken);

        static (string grocery, decimal qty)[] GetItemsFor(Meal meal, NutritionGoal goal)
        {
            // Deterministic variation (use day + meal id) to avoid fully repetitive data.
            var variant = (meal.LocalDate.DayNumber + meal.Id.Value.GetHashCode()) % 2;

            return goal switch
            {
                NutritionGoal.LoseWeight => GetLoseWeightItems(meal.Name, variant),
                NutritionGoal.GainWeight => GetGainWeightItems(meal.Name, variant),
                _ => GetMaintainWeightItems(meal.Name, variant),
            };
        }

        static (string grocery, decimal qty)[] GetLoseWeightItems(string mealName, int variant) => mealName switch
        {
            // Higher protein, more veg, moderate carbs.
            "Breakfast" => variant == 0
                ? [ ("Greek Yogurt (plain)", 200m), ("Blueberries", 120m), ("Honey", 5m) ]
                : [ ("Oats", 45m), ("Milk 2%", 150m), ("Banana", 80m) ],

            "Lunch" => variant == 0
                ? [ ("Chicken Breast (cooked)", 200m), ("Broccoli", 200m), ("Olive Oil", 7m) ]
                : [ ("Turkey Breast (cooked)", 180m), ("Quinoa (cooked)", 140m), ("Spinach", 120m) ],

            "Dinner" => variant == 0
                ? [ ("Salmon", 140m), ("Spinach", 160m), ("Sweet Potato", 140m) ]
                : [ ("Tuna (canned in water)", 160m), ("Lettuce", 150m), ("Olive Oil", 10m) ],

            "Snack" => variant == 0
                ? [ ("Apple", 180m), ("Almonds", 15m) ]
                : [ ("Greek Yogurt (plain)", 170m), ("Blueberries", 80m) ],

            _ => []
        };

        static (string grocery, decimal qty)[] GetMaintainWeightItems(string mealName, int variant) => mealName switch
        {
            // Balanced protein/carbs/fats.
            "Breakfast" => variant == 0
                ? [ ("Oats", 60m), ("Milk 2%", 200m), ("Banana", 100m) ]
                : [ ("Greek Yogurt (plain)", 200m), ("Honey", 10m), ("Blueberries", 100m) ],

            "Lunch" => variant == 0
                ? [ ("Chicken Breast (cooked)", 180m), ("White Rice (cooked)", 180m), ("Broccoli", 150m), ("Olive Oil", 10m) ]
                : [ ("Turkey Breast (cooked)", 170m), ("Quinoa (cooked)", 170m), ("Bell Pepper", 120m), ("Olive Oil", 10m) ],

            "Dinner" => variant == 0
                ? [ ("Egg", 2m), ("Whole Wheat Bread", 80m), ("Tomato", 120m) ]
                : [ ("Salmon", 130m), ("Sweet Potato", 170m), ("Spinach", 120m) ],

            "Snack" => variant == 0
                ? [ ("Greek Yogurt (plain)", 170m), ("Honey", 10m), ("Almonds", 20m) ]
                : [ ("Orange", 160m), ("Walnuts", 15m) ],

            _ => []
        };

        static (string grocery, decimal qty)[] GetGainWeightItems(string mealName, int variant) => mealName switch
        {
            // Higher calories: slightly higher carbs + healthy fats.
            "Breakfast" => variant == 0
                ? [ ("Oats", 80m), ("Whole Milk", 250m), ("Banana", 120m), ("Peanut Butter", 20m) ]
                : [ ("Granola", 80m), ("Greek Yogurt (plain)", 250m), ("Honey", 15m), ("Banana", 100m) ],

            "Lunch" => variant == 0
                ? [ ("Chicken Breast (cooked)", 200m), ("White Rice (cooked)", 240m), ("Olive Oil", 12m), ("Broccoli", 150m) ]
                : [ ("Beef Steak (lean)", 180m), ("Pasta (cooked)", 260m), ("Olive Oil", 10m), ("Tomato", 150m) ],

            "Dinner" => variant == 0
                ? [ ("Egg", 3m), ("Whole Wheat Bread", 130m), ("Avocado", 100m), ("Tomato", 120m) ]
                : [ ("Salmon", 170m), ("Quinoa (cooked)", 240m), ("Olive Oil", 10m), ("Spinach", 120m) ],

            "Snack" => variant == 0
                ? [ ("Protein Bar", 60m), ("Banana", 100m) ]
                : [ ("Dark Chocolate 70%", 25m), ("Almonds", 25m), ("Milk 2%", 200m) ],

            _ => []
        };
    }
}
