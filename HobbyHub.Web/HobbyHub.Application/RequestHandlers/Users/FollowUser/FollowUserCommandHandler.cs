using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.FollowUser;

public class FollowUserCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<FollowUserCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (request.UserId == request.UserToFollowId)
        {
            var error = UserErrors.CannotFollowItself;
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        if (!userExists)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var userToFollow = await repository.UsersRepository.GetUserByIdAsync(request.UserToFollowId, cancellationToken);

        if (userToFollow == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserToFollowId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var userFollows = await repository.UsersRepository.GetUserFollows(request.UserId, cancellationToken);

        if (userFollows.Contains(userToFollow))
        {
            var error = UserErrors.UserIsAlreadyFollowed(request.UserId, request.UserToFollowId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var followId = await repository.UsersRepository.Follow(request.UserId, request.UserToFollowId, cancellationToken);
        
        log.Information($"User with id {request.UserToFollowId} started to follow user with id {request.UserToFollowId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(followId));
    }
}