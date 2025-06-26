using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.AddEventMember;

public class AddEventMemberCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<AddEventMemberCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(AddEventMemberCommand request, CancellationToken cancellationToken)
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

        if (groupEvent.Users.Contains(user))
        {
            var error = EventErrors.UserIsAlreadyAdded(request.UserId, request.EventId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        groupEvent.Users.Add(user);
        await repository.EventsRepository.SaveChanges(cancellationToken);
        
        log.Information($"User with id {user.UserId} added to event with id {groupEvent.EventId} as a member");
        
        return Result<CommandResponse>.Success(new CommandResponse(0));
    }
}