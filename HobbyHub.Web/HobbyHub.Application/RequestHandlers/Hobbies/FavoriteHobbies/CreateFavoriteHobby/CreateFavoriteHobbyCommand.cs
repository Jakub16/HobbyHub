using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;

public class CreateFavoriteHobbyCommand(CreateFavoriteHobbyRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public int HobbyId { get; } = request.HobbyId;
    public bool IsHobbyPredefined { get; } = request.IsHobbyPredefined;
}