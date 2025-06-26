using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;

public class GetUserHobbiesQuery(int userId) : IHobbyHubRequest<Result<ListResponse<HobbyResponse>>>
{
    public int UserId { get; } = userId;
}