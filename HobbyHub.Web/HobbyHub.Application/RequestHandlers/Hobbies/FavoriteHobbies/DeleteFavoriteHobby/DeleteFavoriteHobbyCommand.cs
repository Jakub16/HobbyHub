using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;

public class DeleteFavoriteHobbyCommand(int favoriteHobbyId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int FavoriteHobbyId { get; } = favoriteHobbyId;
}