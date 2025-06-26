using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetTutorials;

public class GetTutorialsQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    ITutorialMapper tutorialMapper,
    ILogger log)
    : IRequestHandler<GetTutorialsQuery, Result<ListResponse<TutorialResponse>>>
{
    public async Task<Result<ListResponse<TutorialResponse>>> 
        Handle(GetTutorialsQuery request, CancellationToken cancellationToken)
    {
        var tutorials = await repository.TutorialsRepository.GetAllTutorials(cancellationToken);
        
        var userIds = tutorials.Select(e => e.CreatedBy).Distinct().ToList();
        var users = await repository.UsersRepository.GetUsersByIds(userIds, cancellationToken);
        
        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        tutorials.ForEach(tutorial =>
        {
            var creator = users.FirstOrDefault(u => u.UserId == tutorial.CreatedBy);
            
            userDictionary.Add(tutorial.TutorialId, mapper.Map<UserSummaryResponse>(creator));
        });
        
        var result = tutorialMapper.Map(tutorials, userDictionary);
        
        log.Information($"Found {result.Count} tutorials");
        
        return Result<ListResponse<TutorialResponse>>.Success(new ListResponse<TutorialResponse>
        {
            Count = result.Count,
            Items = result
        });
    }
}