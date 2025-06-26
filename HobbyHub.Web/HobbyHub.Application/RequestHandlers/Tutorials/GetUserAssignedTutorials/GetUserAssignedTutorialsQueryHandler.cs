using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetUserAssignedTutorials;

public class GetUserAssignedTutorialsQueryHandler(
    IHobbyHubRepository repository,
    ITutorialMapper tutorialMapper,
    IMapper mapper,
    ILogger log)
    : IRequestHandler<GetUserAssignedTutorialsQuery, Result<ListResponse<TutorialResponse>>>
{
    public async Task<Result<ListResponse<TutorialResponse>>> 
        Handle(GetUserAssignedTutorialsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<TutorialResponse>>.Failure(error);
        }
        
        var tutorials = await repository.TutorialsRepository
            .GetUserAssignedTutorials(request.UserId, cancellationToken);
        
        var userIds = tutorials.Select(e => e.CreatedBy).Distinct().ToList();
        var users = await repository.UsersRepository.GetUsersByIds(userIds, cancellationToken);
        
        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        tutorials.ForEach(tutorial =>
        {
            var creator = users.FirstOrDefault(u => u.UserId == tutorial.CreatedBy);
            
            userDictionary.Add(tutorial.TutorialId, mapper.Map<UserSummaryResponse>(creator));
        });
        
        var result = tutorialMapper.Map(tutorials, userDictionary);
        
        log.Information($"Found {result.Count} tutorials assigned to user with id {request.UserId}");
        
        return Result<ListResponse<TutorialResponse>>.Success(new ListResponse<TutorialResponse>
        {
            Count = result.Count,
            Items = result
        });
    }
}