using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;

public class GetFavoriteHobbiesQuery(int userId) : IHobbyHubRequest<Result<ListResponse<FavoriteHobbyResponse>>>
{
    public int UserId { get; } = userId;
}