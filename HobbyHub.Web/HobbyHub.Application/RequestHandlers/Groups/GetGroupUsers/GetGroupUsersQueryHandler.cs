using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;

public class GetGroupUsersQueryHandler(IHobbyHubRepository repository, ILogger log, IMapper mapper)
    : IRequestHandler<GetGroupUsersQuery, Result<ListResponse<UserSummaryResponse>>>
{
    public async Task<Result<ListResponse<UserSummaryResponse>>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFound(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<UserSummaryResponse>>.Failure(error);
        }

        var groupUsers = await repository.GroupsRepository.GetGroupMembers(request.GroupId, cancellationToken);
        
        log.Information($"Found {groupUsers.Count} members for group with id {group.GroupId}");
        
        return Result<ListResponse<UserSummaryResponse>>.Success(new ListResponse<UserSummaryResponse>()
        {
            Items = mapper.Map<List<UserSummaryResponse>>(groupUsers),
            Count = groupUsers.Count
        });
    }
}