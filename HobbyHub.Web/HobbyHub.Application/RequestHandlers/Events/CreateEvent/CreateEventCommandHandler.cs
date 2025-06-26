using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.CreateEvent;

public class CreateEventCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateEventCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFoundCommand(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var groupEvent = new Event()
        {
            Title = request.Title,
            Description = request.Description,
            DateTime = request.DateTime,
            Group = group,
            IsPrivate = true,
            CreatedBy = request.UserId,
            TimeOfCreation = DateTime.Now,
            Address = request.Address
        };
        
        groupEvent.Users.Add(user);

        await repository.EventsRepository.Add(groupEvent, cancellationToken);
        
        if (request.MainPicture != null)
        {
            var pictureUrl = await fileUploader.Send(
                "events/main",
                $"event_{groupEvent.EventId.ToString()}",
                request.MainPicture);

            groupEvent.MainPicturePath = pictureUrl;
        }
        
        await repository.EventsRepository.SaveChanges(cancellationToken);
        
        log.Information($"User with id {request.UserId} created new event with id {groupEvent.EventId} in group with id {request.GroupId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(groupEvent.EventId));
    }
}