using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;

namespace HobbyHub.Application.RequestHandlers.Groups.GetPublicGroups;

public class GetPublicGroupsQuery : IHobbyHubRequest<Result<ListResponse<GroupSummaryResponse>>>
{
    
}