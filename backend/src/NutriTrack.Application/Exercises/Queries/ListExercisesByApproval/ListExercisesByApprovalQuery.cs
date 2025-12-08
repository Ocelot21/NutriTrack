using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Queries.ListExercisesByApproval;

public sealed record ListExercisesByApprovalQuery(bool IsApproved, int Page, int PageSize) : IRequest<ErrorOr<PagedResult<ExerciseResult>>>;
