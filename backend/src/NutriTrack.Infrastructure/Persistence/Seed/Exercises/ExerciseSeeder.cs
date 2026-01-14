using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Infrastructure.Persistence.Seed.Exercises;

public sealed class ExerciseSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public ExerciseSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 80;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var exercises = new (string Name, string? Description, ExerciseCategory Category, decimal CaloriesPerMinute)[]
        {
            ("Walking", "Easy pace", ExerciseCategory.Cardio, 3.5m),
            ("Jogging", "Light jog", ExerciseCategory.Cardio, 7.0m),
            ("Running", "Moderate pace", ExerciseCategory.Cardio, 10.0m),
            ("Cycling (leisure)", "Outdoor or stationary", ExerciseCategory.Cardio, 6.5m),
            ("Cycling (vigorous)", "High intensity", ExerciseCategory.Cardio, 10.5m),
            ("Rowing (moderate)", "Indoor rowing machine", ExerciseCategory.Cardio, 8.5m),
            ("Jump Rope", "Steady pace", ExerciseCategory.HIIT, 12.0m),
            ("Stair Climbing", "Stairs or step machine", ExerciseCategory.Cardio, 9.0m),
            ("Elliptical", "Moderate effort", ExerciseCategory.Cardio, 7.5m),
            ("Swimming (laps)", "Freestyle, moderate", ExerciseCategory.Cardio, 9.5m),

            ("Push-ups", "Bodyweight chest and triceps", ExerciseCategory.Strength, 6.0m),
            ("Pull-ups", "Bodyweight back and biceps", ExerciseCategory.Strength, 7.0m),
            ("Bodyweight Squats", "Lower body", ExerciseCategory.Strength, 5.5m),
            ("Lunges", "Lower body unilateral", ExerciseCategory.Strength, 6.0m),
            ("Plank", "Core hold", ExerciseCategory.Strength, 3.0m),
            ("Bench Press", "Barbell chest press", ExerciseCategory.Strength, 6.5m),
            ("Deadlift", "Barbell hip hinge", ExerciseCategory.Strength, 7.5m),
            ("Back Squat", "Barbell squat", ExerciseCategory.Strength, 7.0m),
            ("Overhead Press", "Barbell or dumbbell press", ExerciseCategory.Strength, 6.0m),
            ("Dumbbell Row", "Upper back pulling", ExerciseCategory.Strength, 6.0m),
            ("Lat Pulldown", "Machine pulldown", ExerciseCategory.Strength, 5.5m),
            ("Leg Press", "Machine squat pattern", ExerciseCategory.Strength, 6.0m),
            ("Leg Extension", "Machine quad isolation", ExerciseCategory.Strength, 4.5m),
            ("Hamstring Curl", "Machine hamstring isolation", ExerciseCategory.Strength, 4.5m),
            ("Bicep Curl", "Dumbbell or barbell", ExerciseCategory.Strength, 3.5m),
            ("Tricep Pushdown", "Cable triceps", ExerciseCategory.Strength, 3.5m),
            ("Calf Raises", "Standing or seated", ExerciseCategory.Strength, 4.0m),

            ("Yoga (vinyasa)", "Flow-based yoga", ExerciseCategory.Flexibility, 4.0m),
            ("Yoga (restorative)", "Gentle stretching", ExerciseCategory.Flexibility, 2.5m),
            ("Stretching", "Full-body mobility and flexibility", ExerciseCategory.Flexibility, 2.5m),
            ("Foam Rolling", "Self-myofascial release", ExerciseCategory.Mobility, 2.0m),
            ("Dynamic Warm-up", "Leg swings, arm circles", ExerciseCategory.Mobility, 3.0m),
            ("Mobility Drill Session", "Hips/ankles/shoulders", ExerciseCategory.Mobility, 3.0m),

            ("HIIT - Burpees", "Intervals", ExerciseCategory.HIIT, 12.5m),
            ("HIIT - Mountain Climbers", "Intervals", ExerciseCategory.HIIT, 10.5m),
            ("HIIT - High Knees", "Intervals", ExerciseCategory.HIIT, 10.0m),
            ("HIIT - Kettlebell Swings", "Intervals", ExerciseCategory.HIIT, 11.0m),
            ("HIIT - Battle Ropes", "Intervals", ExerciseCategory.HIIT, 11.0m),

            ("Hiking", "Moderate trail", ExerciseCategory.Cardio, 6.0m),
            ("Boxing (bag work)", "Heavy bag rounds", ExerciseCategory.Cardio, 9.5m),
            ("Kickboxing", "Class or drills", ExerciseCategory.Cardio, 10.0m),
            ("Dancing", "General", ExerciseCategory.Cardio, 5.5m),
            ("Pilates", "Mat-based", ExerciseCategory.Mobility, 4.0m),
            ("CrossFit (general)", "Mixed modality", ExerciseCategory.HIIT, 11.0m),
        };

        var existingNames = await _dbContext.Exercises
            .Select(e => e.Name)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);

        var toInsert = exercises
            .Where(e => !existingSet.Contains(e.Name))
            .Select(e => Exercise.Create(
                name: e.Name,
                description: e.Description!,
                category: e.Category,
                defaultCaloriesPerMinute: e.CaloriesPerMinute,
                imageUrl: null,
                isApproved: true))
            .ToArray();

        if (toInsert.Length > 0)
        {
            _dbContext.Exercises.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
