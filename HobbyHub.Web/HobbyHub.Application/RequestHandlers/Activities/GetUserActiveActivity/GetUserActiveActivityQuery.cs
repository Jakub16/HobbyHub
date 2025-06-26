using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Responses.Activities;

namespace HobbyHub.Application.RequestHandlers.Activities.GetUserActiveActivity;

public class GetUserActiveActivityQuery(int userId) : IHobbyHubRequest<Result<ActivityResponse?>>
{
    public int UserId { get; } = userId;
}