using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.DeleteEventMember;

public class DeleteEventMemberCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteEventMemberCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteEventMemberCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var groupEvent = await repository.EventsRepository.GetEventById(request.EventId, cancellationToken);

        if (groupEvent == null)
        {
            var error = EventErrors.EventNotFoundCommand(request.EventId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        groupEvent.Users.Remove(user);
        await repository.EventsRepository.SaveChanges(cancellationToken);
        
        log.Information($"Deleted user with id {user.UserId} from event with id {groupEvent.EventId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(user.UserId));
    }
}