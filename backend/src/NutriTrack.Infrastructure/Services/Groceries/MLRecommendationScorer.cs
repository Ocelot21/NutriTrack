using Microsoft.ML;
using NutriTrack.Infrastructure.Services.Groceries.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class MLRecommendationScorer : IDisposable
{
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private bool _isModelTrained;

    public MLRecommendationScorer()
    {
        _mlContext = new MLContext(seed: 42);
        _isModelTrained = false;
    }

    public double Score(GroceryFeatures features)
    {
        if (!_isModelTrained)
        {
            return FallbackHeuristicScore(features);
        }

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<GroceryFeatures, GroceryPrediction>(_model!);
        var prediction = predictionEngine.Predict(features);
        return prediction.Score;
    }

    public void TrainModel(IEnumerable<GroceryFeatures> trainingData, IEnumerable<float> labels)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(
            trainingData.Zip(labels, (features, label) => new TrainingData
            {
                Features = features,
                Label = label
            }));

        var pipeline = _mlContext.Transforms.Concatenate(
                "Features",
                nameof(GroceryFeatures.ProteinPer100),
                nameof(GroceryFeatures.CarbsPer100),
                nameof(GroceryFeatures.FatPer100),
                nameof(GroceryFeatures.CaloriesPer100),
                nameof(GroceryFeatures.PopularityScore),
                nameof(GroceryFeatures.SeasonalityScore),
                nameof(GroceryFeatures.CategoryScore),
                nameof(GroceryFeatures.GoalType),
                nameof(GroceryFeatures.UserAge),
                nameof(GroceryFeatures.UserGender),
                nameof(GroceryFeatures.UserActivityLevel))
            .Append(_mlContext.Regression.Trainers.Sdca(
                labelColumnName: "Label",
                featureColumnName: "Features",
                maximumNumberOfIterations: 100));

        _model = pipeline.Fit(dataView);
        _isModelTrained = true;
    }

    private static double FallbackHeuristicScore(GroceryFeatures features)
    {
        var goalType = (NutritionGoal)features.GoalType;
        
        var macroScore = goalType switch
        {
            NutritionGoal.LoseWeight => 
                (features.ProteinPer100 * 2.0) - 
                (features.CaloriesPer100 * 0.02) - 
                (features.FatPer100 * 0.5) - 
                (features.CarbsPer100 * 0.2),
            
            NutritionGoal.GainWeight => 
                (features.CaloriesPer100 * 0.02) + 
                (features.CarbsPer100 * 0.6) + 
                (features.FatPer100 * 0.8) + 
                (features.ProteinPer100 * 0.8),
            
            _ => 
                (features.ProteinPer100 * 1.0) + 
                (features.CarbsPer100 * 0.3) + 
                (features.FatPer100 * 0.3) - 
                (Math.Abs(features.CaloriesPer100 - 200) * 0.01)
        };

        var score = macroScore + 
                   (features.PopularityScore * 1.5) + 
                   (features.SeasonalityScore * 0.5) + 
                   (features.CategoryScore * 1.0);

        return score;
    }

    public void Dispose()
    {
        (_model as IDisposable)?.Dispose();
    }

    private sealed class TrainingData
    {
        public GroceryFeatures Features { get; set; } = null!;
        public float Label { get; set; }
    }
}
