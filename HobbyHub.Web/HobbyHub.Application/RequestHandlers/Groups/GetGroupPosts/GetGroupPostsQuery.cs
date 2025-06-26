using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupPosts;

public class GetGroupPostsQuery(int groupId) : IHobbyHubRequest<Result<ListResponse<GroupPostResponse>>>
{
    public int GroupId { get; } = groupId;
}