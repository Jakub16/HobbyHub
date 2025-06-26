using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;

namespace HobbyHub.Application.RequestHandlers.Users.FollowUser;

public class FollowUserCommand(FollowUserRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public int UserToFollowId { get; } = request.UserToFollowId;
}