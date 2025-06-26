using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Activities.StartActivity;

public class StartActivityCommandHandler(IHobbyHubRepository repository, ILogger log) 
    : IRequestHandler<StartActivityCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(StartActivityCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        Hobby? hobby = null;
        UserHobby? userHobby = null;

        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        if (request.IsHobbyPredefined)
        {
            hobby = await repository.HobbiesRepository.GetHobbyById(request.HobbyId, cancellationToken);
        }
        else
        {
            userHobby = await repository.UserHobbiesRepository.GetUserHobbyById(request.HobbyId, cancellationToken);
        }
        
        if (hobby == null && userHobby == null)
        {
            var error = HobbyErrors.HobbyNotFoundCommand(request.HobbyId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var activity = new Activity()
        {
            Name = hobby is not null ? hobby.Name : userHobby!.Name,
            StartTime = startTime,
            User = user
        };

        if (hobby is not null)
        {
            activity.Hobby = hobby;
        }
        
        if (userHobby is not null)
        {
            activity.UserHobby = userHobby;
        }

        await repository.ActivitiesRepository.Add(activity, cancellationToken);
        log.Information($"Created activity with id {activity.ActivityId} and StartTime {startTime}");
        
        var post = new Post()
        {
            Activity = activity,
        };

        await repository.PostsRepository.Add(post, cancellationToken);

        return Result<CommandResponse>.Success(new CommandResponse(activity.ActivityId));
    }
}