using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.GetUserSummary;

public class GetUserSummaryQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetUserSummaryQuery, Result<UserSummaryResponse>>
{
    public async Task<Result<UserSummaryResponse>> Handle(GetUserSummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            log.Warning(error.Detail);
            return Result<UserSummaryResponse>.Failure(error);
        }
        
        var userResponse = mapper.Map<UserSummaryResponse>(user);
        
        log.Information($"Found user with id {user.UserId}");
        
        return Result<UserSummaryResponse>.Success(userResponse);
    }
}