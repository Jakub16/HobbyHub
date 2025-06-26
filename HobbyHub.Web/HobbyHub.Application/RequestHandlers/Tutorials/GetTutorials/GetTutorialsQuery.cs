using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Tutorials;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetTutorials;

public class GetTutorialsQuery : IHobbyHubRequest<Result<ListResponse<TutorialResponse>>>
{
    
}