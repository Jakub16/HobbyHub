using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Application.RequestHandlers.Users.SearchUsers;

public class SearchUsersQuery(string keyword) : IHobbyHubRequest<Result<ListResponse<UserSummaryResponse>>>
{
    public string Keyword { get; } = keyword;
}