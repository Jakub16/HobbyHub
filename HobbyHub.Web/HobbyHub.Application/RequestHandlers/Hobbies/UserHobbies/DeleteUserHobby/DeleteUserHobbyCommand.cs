using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;

public class DeleteUserHobbyCommand(int userId, int favoriteHobbyId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = userId;
    public int HobbyId { get; } = favoriteHobbyId;
}