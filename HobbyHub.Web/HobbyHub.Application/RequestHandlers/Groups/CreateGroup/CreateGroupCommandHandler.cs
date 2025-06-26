using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateGroup;

public class CreateGroupCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log) 
    : IRequestHandler<CreateGroupCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var group = new Group()
        {
            Name = request.Name,
            Description = request.Description,
            IsPrivate = request.IsPrivate,
            CreatedBy = request.UserId,
            TimeOfCreation = DateTime.Now
        };
        
        group.Users.Add(user);

        await repository.GroupsRepository.Add(group, cancellationToken);
        
        user.Groups.Add(group);
        
        await repository.GroupsRepository.SaveChanges(cancellationToken);
        
        if (request.MainPicture != null)
        {
            var pictureUrl = await fileUploader.Send(
                "groups/main",
                $"group_{group.GroupId.ToString()}",
                request.MainPicture);

            group.MainPicturePath = pictureUrl;
        }
        
        await repository.GroupsRepository.SaveChanges(cancellationToken);
        
        log.Information($"Created new group with id {group.GroupId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(group.GroupId));
    }
}