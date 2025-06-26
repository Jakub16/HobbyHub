using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetUserGroups;

public class GetUserGroupsQueryHandler(IHobbyHubRepository repository, ILogger log, IMapper mapper)
    : IRequestHandler<GetUserGroupsQuery, Result<ListResponse<GroupSummaryResponse>>>
{
    public async Task<Result<ListResponse<GroupSummaryResponse>>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<GroupSummaryResponse>>.Failure(error);
        }

        var groups = await repository.GroupsRepository.GetUserGroups(request.UserId, cancellationToken);
        
        var result = groups.Select(group =>
        {
            var creator = repository.UsersRepository.GetUserById(group.CreatedBy);

            return new GroupSummaryResponse()
            {
                CreatedBy = mapper.Map<UserSummaryResponse>(creator),
                Description = group.Description,
                GroupId = group.GroupId,
                IsPrivate = group.IsPrivate,
                MainPicturePath = group.MainPicturePath,
                Name = group.Name,
                TimeOfCreation = group.TimeOfCreation
            };
        }).ToList();
        
        log.Information($"Found {groups.Count} groups for user with id {user.UserId}");
        
        return Result<ListResponse<GroupSummaryResponse>>.Success(new ListResponse<GroupSummaryResponse>()
        {
            Items = result,
            Count = result.Count
        });
    }
}