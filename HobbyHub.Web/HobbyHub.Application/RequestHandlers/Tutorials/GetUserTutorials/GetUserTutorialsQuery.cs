using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Tutorials;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetUserTutorials;

public class GetUserTutorialsQuery(int userId) : IHobbyHubRequest<Result<ListResponse<TutorialResponse>>>
{
    public int UserId { get; } = userId;
}