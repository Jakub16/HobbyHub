using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;

namespace HobbyHub.Application.RequestHandlers.Groups.AddGroupMember;

public class AddGroupMemberCommand(int groupId, int userId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = userId;
    public int GroupId { get; } = groupId;
}