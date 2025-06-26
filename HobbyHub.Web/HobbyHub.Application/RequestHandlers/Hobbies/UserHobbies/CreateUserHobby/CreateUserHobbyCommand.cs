using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;

public class CreateUserHobbyCommand(CreateUserHobbyRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public string Name { get; } = request.Name;
    public string? IconType { get; } = request.IconType;
}