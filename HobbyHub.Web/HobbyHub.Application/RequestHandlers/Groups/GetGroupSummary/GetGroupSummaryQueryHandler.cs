using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetGroupSummary;

public class GetGroupSummaryQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetGroupSummaryQuery, Result<GroupSummaryResponse>>
{
    public async Task<Result<GroupSummaryResponse>> Handle(GetGroupSummaryQuery request, CancellationToken cancellationToken)
    {
        var group = await repository.GroupsRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group == null)
        {
            var error = GroupErrors.GroupNotFound(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<GroupSummaryResponse>.Failure(error);
        }
        
        var creator = await repository.UsersRepository.GetUserByIdAsync(group.CreatedBy, cancellationToken);

        var result = new GroupSummaryResponse()
        {
            CreatedBy = mapper.Map<UserSummaryResponse>(creator),
            Description = group.Description,
            GroupId = group.GroupId,
            IsPrivate = group.IsPrivate,
            MainPicturePath = group.MainPicturePath,
            Name = group.Name,
            TimeOfCreation = group.TimeOfCreation
        };
        
        log.Information($"Got group summary for group with id {group.GroupId}");
        
        return Result<GroupSummaryResponse>.Success(result);
    }
}