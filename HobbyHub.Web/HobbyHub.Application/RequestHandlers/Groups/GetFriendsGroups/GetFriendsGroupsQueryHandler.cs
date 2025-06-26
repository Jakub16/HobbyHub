using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetFriendsGroups;

public class GetFriendsGroupsQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetFriendsGroupsQuery, Result<ListResponse<GroupSummaryResponse>>>
{
    public async Task<Result<ListResponse<GroupSummaryResponse>>> Handle(GetFriendsGroupsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            log.Warning(error.Detail);
            return Result<ListResponse<GroupSummaryResponse>>.Failure(error);
        }
        
        var friendsGroups = await repository.GroupsRepository.GetFriendsGroups(request.UserId, cancellationToken);
        
        var creatorsIds = friendsGroups.Select(group => group.CreatedBy).ToList();
        var creators = await repository.UsersRepository.GetUsersByIds(creatorsIds, cancellationToken);
        
        var result = friendsGroups.Select(group =>
        {
            var creator = creators.FirstOrDefault(creator => creator.UserId == group.CreatedBy);

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
        
        log.Information($"Found {result.Count} friends groups for user with id {request.UserId}");
        
        return Result<ListResponse<GroupSummaryResponse>>.Success(new ListResponse<GroupSummaryResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}