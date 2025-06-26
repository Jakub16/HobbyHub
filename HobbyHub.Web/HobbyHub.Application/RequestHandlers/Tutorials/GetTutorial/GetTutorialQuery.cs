using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Responses.Tutorials;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetTutorial;

public class GetTutorialQuery(int tutorialId) : IHobbyHubRequest<Result<TutorialResponse>>
{
    public int TutorialId { get; } = tutorialId;
}