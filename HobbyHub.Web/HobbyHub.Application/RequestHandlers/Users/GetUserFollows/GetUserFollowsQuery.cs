using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Application.RequestHandlers.Users.GetUserFollows;

public class GetUserFollowsQuery(int userId): IHobbyHubRequest<Result<ListResponse<UserSummaryResponse>>>
{
    public int UserId { get; } = userId;
}