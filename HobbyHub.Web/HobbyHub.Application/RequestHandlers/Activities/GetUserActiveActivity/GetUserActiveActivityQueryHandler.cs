using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Responses.Activities;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Activities.GetUserActiveActivity;

public class GetUserActiveActivityQueryHandler(IHobbyHubRepository repository, ILogger log) 
    : IRequestHandler<GetUserActiveActivityQuery, Result<ActivityResponse?>>
{
    public async Task<Result<ActivityResponse?>> Handle(GetUserActiveActivityQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ActivityResponse?>.Failure(error);
        }
        
        var activities = await repository.ActivitiesRepository.GetUserActiveActivities(
            request.UserId, cancellationToken);
        
        switch (activities.Count)
        {
            case 0:
                return Result<ActivityResponse?>.Success(null);
            case > 1:
                return Result<ActivityResponse?>.Failure(ActivityErrors.MoreThanOneActiveActivity(request.UserId));
            default:
            {
                var activeActivity = activities.First();

                return Result<ActivityResponse?>.Success(new ActivityResponse()
                {
                    ActivityId = activeActivity.ActivityId,
                    ActivityName = activeActivity.Name,
                    Distance = activeActivity.Distance,
                    IsDistanceAvailable = activeActivity.IsDistanceAvailable,
                    StartTime = activeActivity.StartTime,
                    EndTime = activeActivity.EndTime,
                    Time = activeActivity.Time,
                    Hobby = new HobbyResponse()
                    {
                        HobbyId = activeActivity.Hobby?.HobbyId ?? activeActivity.UserHobby.UserHobbyId,
                        Name = activeActivity.Hobby?.Name ?? activeActivity.UserHobby.Name,
                        IconType = activeActivity.Hobby?.IconType ?? activeActivity.UserHobby?.IconType ?? null
                    }
                });
            }
        }
    }
}