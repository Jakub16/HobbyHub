using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.UnfollowUser;

public class UnfollowUserCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<UnfollowUserCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var isFollowing =
            await repository.UsersRepository.IsFollowing(request.UserId, request.UserToUnfollowId, cancellationToken);

        if (!isFollowing)
        {
            var error = UserErrors.UserIsNotFollowed(request.UserId, request.UserToUnfollowId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var followId = 
            await repository.UsersRepository.Unfollow(request.UserId, request.UserToUnfollowId, cancellationToken);
        
        log.Information($"User with id {request.UserId} unfollowed user with id {request.UserToUnfollowId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(followId));
    }
}