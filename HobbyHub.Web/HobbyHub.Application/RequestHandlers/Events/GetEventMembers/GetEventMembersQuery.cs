using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Application.RequestHandlers.Events.GetEventMembers;

public class GetEventMembersQuery(int eventId) : IHobbyHubRequest<Result<ListResponse<UserSummaryResponse>>>
{
    public int EventId { get; } = eventId;
}