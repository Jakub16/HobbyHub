using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.AddGroupMember;

public class AddGroupMemberCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<AddGroupMemberCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
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

        if (group.Users.Contains(user))
        {
            var error = GroupErrors.UserIsAlreadyAdded(request.UserId, request.GroupId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        group.Users.Add(user);
        user.Groups.Add(group);

        await repository.GroupsRepository.SaveChanges(cancellationToken);
        
        log.Information($"Added user with id {user.UserId} as a new member of group with id {group.GroupId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(0));
    }
}