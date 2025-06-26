using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Activities.DeleteActivity;

public class DeleteActivityCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteActivityCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await repository.ActivitiesRepository.GetActivityById(request.ActivityId, cancellationToken);

        if (activity == null)
        {
            var error = ActivityErrors.ActivityNotFoundCommand(request.ActivityId);
            log.Warning(error.Detail);
            
            return Result<CommandResponse>.Failure(error);
        }

        await repository.ActivitiesRepository.Delete(activity, cancellationToken);
        
        log.Information($"Successfully deleted activity with id {activity.ActivityId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(activity.ActivityId));
    }
}