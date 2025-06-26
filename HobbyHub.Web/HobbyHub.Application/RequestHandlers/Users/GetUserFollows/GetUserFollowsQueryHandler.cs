using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.GetUserFollows;

public class GetUserFollowsQueryHandler(IHobbyHubRepository repository,IMapper mapper, ILogger log)
    : IRequestHandler<GetUserFollowsQuery, Result<ListResponse<UserSummaryResponse>>>
{
    public async Task<Result<ListResponse<UserSummaryResponse>>> Handle(GetUserFollowsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);
        
        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<UserSummaryResponse>>.Failure(error);
        }

        var userFollows = await repository.UsersRepository.GetUserFollows(request.UserId, cancellationToken);
        
        log.Information($"Found {userFollows.Count} follows for user with id {request.UserId}");
        
        return Result<ListResponse<UserSummaryResponse>>.Success(new ListResponse<UserSummaryResponse>()
        {
            Items = mapper.Map<List<UserSummaryResponse>>(userFollows),
            Count = userFollows.Count
        });
    }
}