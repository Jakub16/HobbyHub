using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Events;

namespace HobbyHub.Application.RequestHandlers.Events.GetGroupEvents;

public class GetGroupEventsQuery(int groupId) : IHobbyHubRequest<Result<ListResponse<EventResponse>>>
{
    public int GroupId { get; } = groupId;
}