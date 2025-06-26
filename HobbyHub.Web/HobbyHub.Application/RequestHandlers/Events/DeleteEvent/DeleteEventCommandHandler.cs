using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.DeleteEvent;

public class DeleteEventCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteEventCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var groupEvent = await repository.EventsRepository.GetEventById(request.EventId, cancellationToken);

        if (groupEvent == null)
        {
            var error = EventErrors.EventNotFoundCommand(request.EventId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        await repository.EventsRepository.Delete(groupEvent, cancellationToken);
        
        log.Information($"Successfully deleted event with id {groupEvent.EventId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(groupEvent.EventId));
    }
}