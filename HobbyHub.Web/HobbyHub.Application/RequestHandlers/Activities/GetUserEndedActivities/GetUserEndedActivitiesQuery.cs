using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Activities;

namespace HobbyHub.Application.RequestHandlers.Activities.GetUserEndedActivities;

public class GetUserEndedActivitiesQuery(int userId) : IHobbyHubRequest<Result<ListResponse<ActivityResponse>>>
{
    public int UserId { get; } = userId;
}