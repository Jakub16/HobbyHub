using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.Contract.Common;

namespace HobbyHub.Application.RequestHandlers.Activities.StartActivity;

public class StartActivityCommand(StartActivityRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public int HobbyId { get; } = request.HobbyId;
    public bool IsHobbyPredefined { get; } = request.IsHobbyPredefined;
}