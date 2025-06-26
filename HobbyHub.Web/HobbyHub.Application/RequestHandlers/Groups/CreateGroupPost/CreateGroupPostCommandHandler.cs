using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateGroupPost;

public class CreateGroupPostCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateGroupPostCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateGroupPostCommand request, CancellationToken cancellationToken)
    {
        var timeOfCreation = DateTime.UtcNow;
        
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFoundCommand(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var groupPost = new GroupPost()
        {
            User = user,
            Group = group,
            Content = request.Content,
            DateTime = timeOfCreation
        };

        await repository.GroupPostsRepository.Add(groupPost, cancellationToken);
        await repository.GroupPostsRepository.SaveChanges(cancellationToken);
        
        request.Pictures?.ForEach(x => AddPicture(x).GetAwaiter().GetResult());
        
        log.Information($"Created GroupPost with id {groupPost.GroupPostId} with {request.Pictures?.Count} pictures");
        
        return Result<CommandResponse>.Success(new CommandResponse(groupPost.GroupPostId));
        
        async Task AddPicture(IFormFile requestPicture)
        {
            var groupPostPicture = new GroupPostPicture()
            {
                GroupPost = groupPost
            };

            await repository.GroupPostPicturesRepository.Add(groupPostPicture, cancellationToken);
            await repository.GroupPostPicturesRepository.SaveChanges(cancellationToken);
            
            var pictureUrl = await fileUploader.Send(
                "groups/posts",
                $"group_{group.GroupId.ToString()}/post_{groupPost.GroupPostId.ToString()}/groupPostPicture_{groupPostPicture.GroupPostPictureId}",
                requestPicture);

            var foundGroupPostPicture =
                await repository.GroupPostPicturesRepository.GetGroupPostPictureById(groupPostPicture.GroupPostPictureId,
                    cancellationToken);

            foundGroupPostPicture.PathToPicture = pictureUrl;
            await repository.GroupPostPicturesRepository.SaveChanges(cancellationToken);
        }
    }
}