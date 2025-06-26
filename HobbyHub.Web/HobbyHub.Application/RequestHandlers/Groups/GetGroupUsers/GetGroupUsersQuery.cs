using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;

public class GetGroupUsersQuery(int groupId) : IHobbyHubRequest<Result<ListResponse<UserSummaryResponse>>>
{
    public int GroupId { get; } = groupId;
}