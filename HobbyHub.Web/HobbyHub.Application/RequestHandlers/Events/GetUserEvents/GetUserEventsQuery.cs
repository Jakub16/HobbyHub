using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Events;

namespace HobbyHub.Application.RequestHandlers.Events.GetUserEvents;

public class GetUserEventsQuery(int userId): IHobbyHubRequest<Result<ListResponse<EventResponse>>>
{
    public int UserId { get; } = userId;
}