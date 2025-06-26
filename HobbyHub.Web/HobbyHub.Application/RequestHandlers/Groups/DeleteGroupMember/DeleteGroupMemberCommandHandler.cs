using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.DeleteGroupMember;

public class DeleteGroupMemberCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteGroupMemberCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFoundCommand(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var isUserGroupMember = await repository.GroupsRepository
            .IsUserGroupMember(request.GroupId, user, cancellationToken);

        if (!isUserGroupMember)
        {
            var error = GroupErrors.UserIsNotAGroupMember(request.UserId, request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        group.Users.Remove(user);
        await repository.GroupsRepository.SaveChanges(cancellationToken);
        
        log.Information($"Deleted user with id {user.UserId} from group with id {group.GroupId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(user.UserId));
    }
}