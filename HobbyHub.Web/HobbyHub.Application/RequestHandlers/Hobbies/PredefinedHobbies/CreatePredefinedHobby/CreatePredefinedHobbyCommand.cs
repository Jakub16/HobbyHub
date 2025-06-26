using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;

public class CreatePredefinedHobbyCommand(CreateHobbyRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public string Name { get; } = request.Name;
    public string? IconType { get; } = request.IconType;
}