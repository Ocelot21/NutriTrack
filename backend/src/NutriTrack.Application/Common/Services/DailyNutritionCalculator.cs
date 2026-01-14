using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Application.Common.Services;

public static class DailyNutritionCalculator
{
    public static DailyNutritionTargets CalculateGoals(DailyNutritionContext context)
    {
        double bmr;
        if (context.Gender == Gender.Male)
        {
            bmr = 10 * (double)context.WeightKg + 6.25 * (double)context.HeightCm - 5 * context.AgeYears + 5;
        }
        else
        {
            bmr = 10 * (double)context.WeightKg + 6.25 * (double)context.HeightCm - 5 * context.AgeYears - 161;
        }

        double activityMultiplier = context.ActivityLevel switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.Light => 1.375,
            ActivityLevel.Moderate => 1.55,
            ActivityLevel.Active => 1.725,
            ActivityLevel.VeryActive => 1.9,
            _ => 1.2
        };

        var tdee = bmr * activityMultiplier;
        var tdeeDecimal = (decimal)tdee;

        decimal goalMultiplier = context.Goal switch
        {
            NutritionGoal.LoseWeight => 0.80m,
            NutritionGoal.MaintainWeight => 1.00m,
            NutritionGoal.GainWeight => 1.15m,
            _ => 1.00m
        };

        int calorieGoal = (int)Math.Round(tdeeDecimal * goalMultiplier, 0);

        (decimal proteinRatio, decimal carbRatio, decimal fatRatio) = context.Goal switch
        {
            NutritionGoal.LoseWeight => (0.30m, 0.40m, 0.30m),
            NutritionGoal.MaintainWeight => (0.20m, 0.50m, 0.30m),
            NutritionGoal.GainWeight => (0.20m, 0.50m, 0.30m),
            _ => (0.20m, 0.50m, 0.30m)
        };

        var proteinGoal = Math.Round(calorieGoal * proteinRatio / 4m, 0);
        var carbGoal = Math.Round(calorieGoal * carbRatio / 4m, 0);
        var fatGoal = Math.Round(calorieGoal * fatRatio / 9m, 0);

        return new DailyNutritionTargets(
            calorieGoal,
            proteinGoal,
            fatGoal,
            carbGoal  
        );
    }
}
