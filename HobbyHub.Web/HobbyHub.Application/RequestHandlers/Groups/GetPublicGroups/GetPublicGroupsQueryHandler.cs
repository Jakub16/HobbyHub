using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetPublicGroups;

public class GetPublicGroupsQueryHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<GetPublicGroupsQuery, Result<ListResponse<GroupSummaryResponse>>>
{
    public async Task<Result<ListResponse<GroupSummaryResponse>>> Handle(GetPublicGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await repository.GroupsRepository.GetPublicGroups(cancellationToken);
        
        log.Information($"Found {groups.Count} public groups");
        
        var result = groups.Select(group => new GroupSummaryResponse()
        {
            CreatedBy = null,
            Description = group.Description,
            GroupId = group.GroupId,
            IsPrivate = group.IsPrivate,
            MainPicturePath = group.MainPicturePath,
            Name = group.Name,
            TimeOfCreation = group.TimeOfCreation
        }).ToList();
        
        return Result<ListResponse<GroupSummaryResponse>>.Success(new ListResponse<GroupSummaryResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}