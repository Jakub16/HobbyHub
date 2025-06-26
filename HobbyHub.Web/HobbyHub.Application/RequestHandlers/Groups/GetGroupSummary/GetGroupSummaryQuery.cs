using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Responses.Groups;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupSummary;

public class GetGroupSummaryQuery(int groupId) : IHobbyHubRequest<Result<GroupSummaryResponse>>
{
    public int GroupId { get; } = groupId;
}