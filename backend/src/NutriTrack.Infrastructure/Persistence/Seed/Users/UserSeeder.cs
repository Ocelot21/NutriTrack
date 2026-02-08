using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.Users;

public sealed class UserSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public UserSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 60;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var roles = await _dbContext.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync(cancellationToken);

        var roleByName = roles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);

        if (!roleByName.TryGetValue("Admin", out var adminRoleId))
        {
            throw new InvalidOperationException("Admin role not found, run RoleSeeder first!");
        }

        if (!roleByName.TryGetValue("User", out var userRoleId))
        {
            throw new InvalidOperationException("User role not found, run RoleSeeder first!");
        }

        var existingUsernames = await _dbContext.Users
            .Select(u => u.Username.Value)
            .ToListAsync(cancellationToken);

        var existingUsernameSet = new HashSet<string>(existingUsernames, StringComparer.OrdinalIgnoreCase);

        const string defaultTimeZoneId = "UTC";
        const string sharedPassword = "Password123!";
        const string userPassword = "user";
        const string mobilePassword = "mobile";
        const string adminPassword = "admin";
        const string desktopPassword = "desktop";

        var sharedPasswordHash = BCrypt.Net.BCrypt.HashPassword(sharedPassword);
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword(userPassword);
        var mobilePasswordHash = BCrypt.Net.BCrypt.HashPassword(mobilePassword);
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
        var desktopPasswordHash = BCrypt.Net.BCrypt.HashPassword(desktopPassword);

        var users = new (string FirstName, string LastName, string Username, string Email, RoleId RoleId, bool IsEmailVerified, string TimeZoneId, string? CountryIso2,
            Gender Gender, DateOnly? Birthdate, decimal? HeightCm, decimal? WeightKg, ActivityLevel ActivityLevel, NutritionGoal NutritionGoal, bool MarkHealthProfileCompleted)[]
        {
            ("Desktop", "User", "desktop", "desktop@nutritrack.local", adminRoleId, true, defaultTimeZoneId, "US",
                Gender.Male, new DateOnly(1996, 5, 12), 182m, 82m, ActivityLevel.Moderate, NutritionGoal.MaintainWeight, true),

            ("Mobile", "User", "mobile", "mobile@nutritrack.local", userRoleId, true, defaultTimeZoneId, "GB",
                Gender.Female, new DateOnly(2000, 3, 8), 168m, 60m, ActivityLevel.Active, NutritionGoal.LoseWeight, true),

            ("Regular", "User", "user", "user@nutritrack.local", userRoleId, true, defaultTimeZoneId, "DE",
                Gender.Male, new DateOnly(1995, 6, 15), 178m, 85m, ActivityLevel.Sedentary, NutritionGoal.LoseWeight, true),

            ("System", "Admin", "admin", "admin@nutritrack.local", adminRoleId, true, defaultTimeZoneId, "CA",
                Gender.Male, new DateOnly(1988, 11, 2), 175m, 78m, ActivityLevel.Light, NutritionGoal.MaintainWeight, true),

            ("Ava", "Brown", "avabrown", "ava.brown@nutritrack.local", userRoleId, false, defaultTimeZoneId, "FR",
                Gender.Female, new DateOnly(1993, 7, 19), 162m, 55m, ActivityLevel.VeryActive, NutritionGoal.GainWeight, true),

            ("Liam", "Chen", "liamchen", "liam.chen@nutritrack.local", userRoleId, false, defaultTimeZoneId, "JP",
                Gender.Male, new DateOnly(1999, 1, 27), 179m, 90m, ActivityLevel.Moderate, NutritionGoal.LoseWeight, true),

            ("Noah", "Johnson", "noah.johnson", "noah.johnson@nutritrack.local", userRoleId, true, defaultTimeZoneId, "AU",
                Gender.Male, new DateOnly(2002, 9, 3), 171m, 68m, ActivityLevel.Active, NutritionGoal.MaintainWeight, true),

            ("Mia", "Lopez", "mia_lopez", "mia.lopez@nutritrack.local", userRoleId, true, defaultTimeZoneId, "ES",
                Gender.Female, new DateOnly(1990, 2, 14), 158m, 62m, ActivityLevel.Light, NutritionGoal.LoseWeight, true),

            ("Oliver", "Khan", "oliver.khan", "oliver.khan@nutritrack.local", userRoleId, false, defaultTimeZoneId, "SE",
                Gender.Male, new DateOnly(1995, 12, 21), 188m, 92m, ActivityLevel.Sedentary, NutritionGoal.GainWeight, true),

            ("Sophia", "Ivanova", "sophia.ivanova", "sophia.ivanova@nutritrack.local", userRoleId, false, defaultTimeZoneId, "IT",
                Gender.Female, new DateOnly(1985, 6, 30), 170m, 70m, ActivityLevel.Moderate, NutritionGoal.MaintainWeight, true),

            ("Ethan", "Ng", "ethan_ng", "ethan.ng@nutritrack.local", userRoleId, false, defaultTimeZoneId, null,
                Gender.Male, new DateOnly(1998, 4, 5), 180m, 76m, ActivityLevel.VeryActive, NutritionGoal.MaintainWeight, true),
        };

        var toAdd = new List<User>();

        foreach (var u in users)
        {
            if (existingUsernameSet.Contains(u.Username))
            {
                continue;
            }

            var passwordHash = u.Username.ToLowerInvariant() switch
            {
                "user" => userPasswordHash,
                "desktop" => desktopPasswordHash,
                "admin" => adminPasswordHash,
                "mobile" => mobilePasswordHash,
                _ => sharedPasswordHash
            };

            var user = User.Create(
                firstName: u.FirstName,
                lastName: u.LastName,
                username: u.Username,
                email: u.Email,
                passwordHash: passwordHash,
                timeZoneId: u.TimeZoneId,
                roleId: u.RoleId,
                countryIso2: u.CountryIso2,
                isEmailVerified: u.IsEmailVerified);

            user.UpdateHealthProfile(
                gender: u.Gender,
                birthdate: u.Birthdate,
                heightCm: u.HeightCm,
                weightKg: u.WeightKg,
                activityLevel: u.ActivityLevel,
                nutritionGoal: u.NutritionGoal);

            if (u.MarkHealthProfileCompleted)
            {
                user.MarkHealthProfileCompleted();
            }

            toAdd.Add(user);
        }

        if (toAdd.Count > 0)
        {
            _dbContext.Users.AddRange(toAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
