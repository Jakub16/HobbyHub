using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;

namespace HobbyHub.Application.RequestHandlers.Tutorials.AssignTutorialToUser;

public class AssignTutorialToUserCommand(int userId, int tutorialId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = userId;
    public int TutorialId { get; } = tutorialId;
}