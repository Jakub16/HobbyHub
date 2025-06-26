using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;

namespace HobbyHub.Application.RequestHandlers.Users.UnfollowUser;

public class UnfollowUserCommand(UnfollowUserRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public int UserToUnfollowId { get; } = request.UserToUnfollowId;
}