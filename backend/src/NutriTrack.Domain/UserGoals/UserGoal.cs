using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.UserGoals;

public sealed class UserGoal : AggregateRoot<UserGoalId>
{
    private const decimal DefaultCompletionToleranceKg = 2m;

    public UserId UserId { get; private set; }
    public User User { get; private set; } = null!;

    public NutritionGoal Type { get; private set; }
    public UserGoalStatus Status { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly TargetDate { get; private set; }

    public decimal StartWeightKg { get; private set; }
    public decimal TargetWeightKg { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? FailedAtUtc { get; private set; }

    private UserGoal() : base()
    {
    }

    private UserGoal(
        UserGoalId id,
        UserId userId,
        NutritionGoal type,
        DateOnly startDate,
        DateOnly targetDate,
        decimal startWeightKg,
        decimal targetWeightKg) : base(id)
    {
        UserId = userId;
        Type = type;
        StartDate = startDate;
        TargetDate = targetDate;
        StartWeightKg = startWeightKg;
        TargetWeightKg = targetWeightKg;
        Status = UserGoalStatus.InProgress;
    }

    public static UserGoal Create(
        UserId userId,
        NutritionGoal type,
        DateOnly startDate,
        DateOnly targetDate,
        decimal startWeightKg,
        decimal targetWeightKg)
    {
        if (targetDate < startDate)
        {
            throw new ArgumentException("Target date cannot be before start date.", nameof(targetDate));
        }

        if (startWeightKg <= 0 || targetWeightKg <= 0)
        {
            throw new ArgumentException("Weights must be positive.");
        }

        switch (type)
        {
            case NutritionGoal.LoseWeight when targetWeightKg >= startWeightKg:
                throw new ArgumentException("Target weight must be less than start weight for LoseWeight goal.");
            case NutritionGoal.GainWeight when targetWeightKg <= startWeightKg:
                throw new ArgumentException("Target weight must be greater than start weight for GainWeight goal.");
            case NutritionGoal.MaintainWeight:
                break;
        }

        var id = new UserGoalId(Guid.NewGuid());

        return new UserGoal(
            id,
            userId,
            type,
            startDate,
            targetDate,
            startWeightKg,
            targetWeightKg);
    }


    public bool EvaluateProgress(
        decimal currentWeightKg,
        DateOnly currentDate,
        DateTime utcNow,
        decimal? toleranceKg = null)
    {
        if (Status is UserGoalStatus.Completed or UserGoalStatus.Failed or UserGoalStatus.Cancelled)
        {
            return false;
        }

        var tolerance = toleranceKg ?? DefaultCompletionToleranceKg;

        var isCompleted = Type switch
        {
            NutritionGoal.LoseWeight =>
                currentWeightKg <= TargetWeightKg,
            NutritionGoal.GainWeight =>
                currentWeightKg >= TargetWeightKg,
            NutritionGoal.MaintainWeight =>
                Math.Abs(currentWeightKg - StartWeightKg) <= tolerance,
            _ => false
        };

        if (isCompleted)
        {
            Status = UserGoalStatus.Completed;
            CompletedAtUtc = utcNow;
            SetModified(utcNow, null);
            return true;
        }

        if (currentDate > TargetDate)
        {
            Status = UserGoalStatus.Failed;
            FailedAtUtc = utcNow;
            SetModified(utcNow, null);
            return true;
        }

        return false;
    }

    public void Cancel(DateTime utcNow)
    {
        if (Status is UserGoalStatus.Completed or UserGoalStatus.Failed or UserGoalStatus.Cancelled)
        {
            return;
        }

        Status = UserGoalStatus.Cancelled;
        SetModified(utcNow, null);
    }
}
