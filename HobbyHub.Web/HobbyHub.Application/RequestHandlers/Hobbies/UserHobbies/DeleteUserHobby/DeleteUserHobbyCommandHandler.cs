using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;

public class DeleteUserHobbyCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteUserHobbyCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteUserHobbyCommand request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);
        
        if (!userExists)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var userHobbyExists = await repository.UserHobbiesRepository.UserHobbyExists(request.HobbyId, cancellationToken);

        if (!userHobbyExists)
        {
            var error = HobbyErrors.HobbyNotFoundCommand(request.HobbyId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        await repository.UserHobbiesRepository.Delete(request.HobbyId, cancellationToken);
        
        log.Information($"Successfully deleted user hobby with id {request.HobbyId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(request.HobbyId));
    }
}