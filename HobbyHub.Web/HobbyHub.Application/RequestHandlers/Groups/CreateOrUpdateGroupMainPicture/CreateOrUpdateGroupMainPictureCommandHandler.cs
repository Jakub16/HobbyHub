using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;

public class CreateOrUpdateGroupMainPictureCommandHandler(
    IHobbyHubRepository repository,
    IFileUploader fileUploader,
    ILogger log)
    : IRequestHandler<CreateOrUpdateGroupMainPictureCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateOrUpdateGroupMainPictureCommand request, CancellationToken cancellationToken)
    {
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFoundCommand(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var pictureUrl = await fileUploader.Send(
            "groups/main",
            $"group_{group.GroupId.ToString()}",
            request.Picture);

        group.MainPicturePath = pictureUrl;
        await repository.GroupsRepository.SaveChanges(cancellationToken);
        
        log.Information($"Created or updated main picture for group with id {group.GroupId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(group.GroupId));
    }
}