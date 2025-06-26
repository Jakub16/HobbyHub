using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Activities;

namespace HobbyHub.Application.RequestHandlers.Activities.DeleteActivity;

public class DeleteActivityCommand(int activityId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int ActivityId { get; } = activityId;
}