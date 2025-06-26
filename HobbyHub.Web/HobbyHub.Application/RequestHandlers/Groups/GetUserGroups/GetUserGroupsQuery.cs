using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;

namespace HobbyHub.Application.RequestHandlers.Groups.GetUserGroups;

public class GetUserGroupsQuery(int userId) : IHobbyHubRequest<Result<ListResponse<GroupSummaryResponse>>>
{
    public int UserId { get; } = userId;
}