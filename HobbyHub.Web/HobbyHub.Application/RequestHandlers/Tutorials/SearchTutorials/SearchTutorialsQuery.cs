using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Contract.Responses.Tutorials;

namespace HobbyHub.Application.RequestHandlers.Tutorials.SearchTutorials;

public class SearchTutorialsQuery(SearchTutorialsFilters filters) 
    : IHobbyHubRequest<Result<PagedResponse<ListResponse<TutorialResponse>>>>
{
    public SearchTutorialsFilters Filters { get; } = filters;
}