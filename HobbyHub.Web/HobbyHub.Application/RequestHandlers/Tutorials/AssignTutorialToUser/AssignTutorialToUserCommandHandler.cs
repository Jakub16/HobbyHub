using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.AssignTutorialToUser;

public class AssignTutorialToUserCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<AssignTutorialToUserCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(AssignTutorialToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var tutorial = await repository.TutorialsRepository.GetTutorialById(request.TutorialId, cancellationToken);
        
        if (tutorial == null)
        {
            var error = TutorialErrors.TutorialNotFoundCommand(request.TutorialId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        tutorial.Users.Add(user);
        await repository.SaveChanges(cancellationToken);
        
        log.Information($"Assigned tutorial with id {request.TutorialId} to user with id {request.UserId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(tutorial.TutorialId));
    }
}