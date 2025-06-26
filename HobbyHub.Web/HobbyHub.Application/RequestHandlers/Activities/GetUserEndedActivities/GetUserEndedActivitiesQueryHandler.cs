using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Activities;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Activities.GetUserEndedActivities;

public class GetUserEndedActivitiesQueryHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<GetUserEndedActivitiesQuery, Result<ListResponse<ActivityResponse>>>
{
    public async Task<Result<ListResponse<ActivityResponse>>> Handle(GetUserEndedActivitiesQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<ActivityResponse>>.Failure(error);
        }
        
        var userEndedActivities = await repository.ActivitiesRepository.GetUserEndedActivities(
            request.UserId, cancellationToken);

        var result = userEndedActivities.Select(activity => new ActivityResponse()
        {
            ActivityId = activity.ActivityId,
            ActivityName = activity.Name,
            Distance = activity.Distance,
            IsDistanceAvailable = activity.IsDistanceAvailable,
            StartTime = activity.StartTime,
            EndTime = activity.EndTime,
            Time = activity.Time,
            Hobby = new HobbyResponse()
            {
                HobbyId = activity.Hobby?.HobbyId ?? activity.UserHobby.UserHobbyId,
                Name = activity.Hobby?.Name ?? activity.UserHobby.Name,
                IconType = activity.Hobby?.IconType ?? activity.UserHobby?.IconType ?? null
            }
        }).ToList();
        
        log.Information($"Found {result.Count} ended activities for user with id {request.UserId}");
        
        return Result<ListResponse<ActivityResponse>>.Success(new ListResponse<ActivityResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}