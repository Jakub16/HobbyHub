using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupPosts;

public class GetGroupPostsQueryHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<GetGroupPostsQuery, Result<ListResponse<GroupPostResponse>>>
{
    public async Task<Result<ListResponse<GroupPostResponse>>> Handle(GetGroupPostsQuery request, CancellationToken cancellationToken)
    {
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFound(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<GroupPostResponse>>.Failure(error);
        }

        var groupPosts = await repository.GroupPostsRepository.GetGroupPosts(request.GroupId, cancellationToken);

        var result = groupPosts.Select(groupPost =>
        {
            return new GroupPostResponse()
            {
                GroupPostId = groupPost.GroupPostId,
                Content = groupPost.Content,
                DateTime = groupPost.DateTime,
                PathsToPictures = groupPost.GroupPostPictures
                    .Select(groupPostPicture => groupPostPicture.PathToPicture)
                    .ToList(),
                UserSummary = new UserSummaryResponse()
                {
                    UserId = groupPost.User.UserId,
                    Name = groupPost.User.Name,
                    Surname = groupPost.User.Surname,
                    ProfilePicturePath = groupPost.User.ProfilePicturePath
                }
            };
        }).ToList();
        
        log.Information($"Found {result.Count} posts for group with id {request.GroupId}");
        
        return Result<ListResponse<GroupPostResponse>>.Success(new ListResponse<GroupPostResponse>()
        {
            Items = result,
            Count = result.Count
        });
    }
}