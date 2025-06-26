using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Application.RequestHandlers.Users.GetUserSummary;

public class GetUserSummaryQuery(int userId) : IHobbyHubRequest<Result<UserSummaryResponse>>
{
    public int UserId { get; } = userId;
}