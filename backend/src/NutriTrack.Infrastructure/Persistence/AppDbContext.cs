using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.Groceries;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options, 
            IDateTimeProvider dateTimeProvider, 
            ICurrentUserService currentUserService
            )
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Grocery> Groceries { get; set; } = null!;
        public DbSet<Meal> Meals { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<UserExerciseLog> UserExerciseLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = _dateTimeProvider.UtcNow;
            var userId = _currentUserService.UserId;

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(now, userId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetModified(now, userId);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}