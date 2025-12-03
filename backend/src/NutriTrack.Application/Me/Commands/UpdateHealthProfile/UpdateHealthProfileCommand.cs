using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Commands.UpdateHealthProfile;

public record UpdateHealthProfileCommand(
    UserId UserId,
    Gender? Gender,
    DateOnly? Birthdate,
    decimal? HeightCm,
    decimal? WeightKg,
    ActivityLevel? ActivityLevel,
    NutritionGoal? NutritionGoal
) : IRequest<ErrorOr<UserResult>>;