using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Commands.ShareGoalProgressSnapshot;

public sealed class ShareGoalProgressSnapshotCommandHandler : IRequestHandler<ShareGoalProgressSnapshotCommand, ErrorOr<Unit>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IGoalProgressSnapshotRepository _goalProgressSnapshotRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShareGoalProgressSnapshotCommandHandler(
        ISocialPostRepository socialPostRepository,
        IGoalProgressSnapshotRepository goalProgressSnapshotRepository,
        IUserGoalRepository userGoalRepository,
        IWeightHistoryRepository weightHistoryRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _socialPostRepository = socialPostRepository;
        _goalProgressSnapshotRepository = goalProgressSnapshotRepository;
        _userGoalRepository = userGoalRepository;
        _weightHistoryRepository = weightHistoryRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ShareGoalProgressSnapshotCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        if (await _userRepository.GetByIdAsync(userId, cancellationToken) is not User user)
        {
            return Errors.Users.NotFound;
        }

        var userGoalId = new UserGoalId(request.UserGoalId);
        var userGoal = await _userGoalRepository.GetByIdAsync(userGoalId, cancellationToken);
        if (userGoal is null)
        {
            return Errors.UserGoals.NotFound;
        }

        var snapshotDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Build points from weight history entries for the goal range
        var weightEntries = await _weightHistoryRepository.GetInRangeForUser(userId, userGoal.StartDate, snapshotDate, cancellationToken);
        var points = weightEntries
            .OrderBy(e => e.Date)
            .Select(e => (e.Date, e.WeightKg));

        var snapshot = GoalProgressSnapshot.Create(
            userGoal, 
            user.WeightKg is null ? userGoal.StartWeightKg : user.WeightKg.Value,
            snapshotDate,
            points);

        await _goalProgressSnapshotRepository.AddAsync(snapshot, cancellationToken);

        var visibility = request.Visibility.HasValue ? (PostVisibility)request.Visibility.Value : PostVisibility.Public;
        var post = SocialPost.ShareGoalProgressSnapshot(userId, snapshot.Id, request.Caption, visibility);

        await _socialPostRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}