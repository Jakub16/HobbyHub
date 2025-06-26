using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.SearchTutorials;

public class SearchTutorialsQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    ITutorialMapper tutorialMapper,
    ILogger log)
    : IRequestHandler<SearchTutorialsQuery, Result<PagedResponse<ListResponse<TutorialResponse>>>>
{
    public async Task<Result<PagedResponse<ListResponse<TutorialResponse>>>> 
        Handle(SearchTutorialsQuery request, CancellationToken cancellationToken)
    {
        var tutorials = await repository.TutorialsRepository.SearchTutorials(request.Filters, cancellationToken);
        
        var userIds = tutorials.Tutorials.Select(e => e.CreatedBy).Distinct().ToList();
        var users = await repository.UsersRepository.GetUsersByIds(userIds, cancellationToken);
        
        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        tutorials.Tutorials.ForEach(tutorial =>
        {
            var creator = users.FirstOrDefault(u => u.UserId == tutorial.CreatedBy);
            
            userDictionary.Add(tutorial.TutorialId, mapper.Map<UserSummaryResponse>(creator));
        });
        
        var result = tutorialMapper.Map(tutorials.Tutorials, userDictionary);
        
        log.Information($"Found {result.Count} tutorials for page {request.Filters.PageNumber}");
        
        return Result<PagedResponse<ListResponse<TutorialResponse>>>.Success(
            new PagedResponse<ListResponse<TutorialResponse>>
        {
            Data = new ListResponse<TutorialResponse>()
            {
                Count = result.Count,
                Items = result
            },
            PageNumber = request.Filters.PageNumber,
            PageSize = request.Filters.PageSize,
            TotalPages = tutorials.TotalPages,
            TotalRecords = tutorials.TotalRecords
        });
    }
}