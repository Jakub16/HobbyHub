using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;

public class GetUserHobbiesQueryHandler(IHobbyHubRepository repository, ILogger log, IMapper mapper)
    : IRequestHandler<GetUserHobbiesQuery, Result<ListResponse<HobbyResponse>>>
{
    public async Task<Result<ListResponse<HobbyResponse>>> Handle(GetUserHobbiesQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<HobbyResponse>>.Failure(error);
        }
        
        var userHobbies =
            await repository.UserHobbiesRepository.GetUserHobbiesByUserId(request.UserId, cancellationToken);
        
        var count = userHobbies.Count;

        var items = mapper.Map<List<HobbyResponse>>(userHobbies);
        
        log.Information($"Found {userHobbies} user hobbies for user with id {request.UserId}");
        
        return Result<ListResponse<HobbyResponse>>.Success(new ListResponse<HobbyResponse>()
        {
            Items = items,
            Count = count
        });
    }
}