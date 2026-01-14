using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common.Events;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Countries;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.WeightHistory;
using NutriTrack.Infrastructure.Persistence.TwoFactor;

namespace NutriTrack.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public AppDbContext(
            DbContextOptions<AppDbContext> options, 
            IDateTimeProvider dateTimeProvider, 
            ICurrentUserService currentUserService,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Grocery> Groceries { get; set; } = null!;
        public DbSet<Meal> Meals { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<UserExerciseLog> UserExerciseLogs { get; set; } = null!;
        public DbSet<UserGoal> UserGoals { get; set; } = null!;
        public DbSet<WeightHistoryEntry> WeightHistoryEntries { get; set; } = null!;
        public DbSet<ActivityLevelHistoryEntry> ActivityLevelHistoryEntries { get; set; } = null!;
        public DbSet<Achievement> Achievements { get; set; } = null!;
        public DbSet<UserAchievement> UserAchievements { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<PendingTotpSetupEntity> PendingTotpSetups { get; set; } = null!;
        public DbSet<GoalProgressSnapshot> GoalProgressSnapshots { get; set; } = null!;
        public DbSet<DailyOverviewSnapshot> DailyOverviewSnapshots { get; set; } = null!;
        public DbSet<SocialPost> SocialPosts { get; set; } = null!;
        public DbSet<ReportRun> ReportRuns { get; set; } = null!;

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

            var entitiesWithEvents = ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);


            if (domainEvents.Count > 0)
            {
                await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

                foreach (var entity in entitiesWithEvents)
                {
                    entity.ClearDomainEvents();
                }
            }

            return result;
        }

    }
}