using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.UpdateEvent;

public class UpdateEventCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<UpdateEventCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var groupEvent = await repository.EventsRepository.GetEventById(request.EventId, cancellationToken);

        if (groupEvent == null)
        {
            var error = EventErrors.EventNotFoundCommand(request.EventId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        groupEvent.Title = request.Title;
        groupEvent.Description = request.Description;
        groupEvent.DateTime = request.DateTime;
        groupEvent.Address = request.Address;

        await repository.EventsRepository.SaveChanges(cancellationToken);
        
        if (request.MainPicture != null)
        {
            var pictureUrl = await fileUploader.Send(
                "events/main",
                $"event_{groupEvent.EventId.ToString()}",
                request.MainPicture);

            groupEvent.MainPicturePath = pictureUrl;
        }
        
        log.Information($"Updated event with id {groupEvent.EventId}");
        
        await repository.EventsRepository.SaveChanges(cancellationToken);
        
        return Result<CommandResponse>.Success(new CommandResponse(groupEvent.EventId));
    }
}