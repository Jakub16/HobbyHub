using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.GetTutorial;

public class GetTutorialQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    ITutorialMapper tutorialMapper,
    ILogger log)
    : IRequestHandler<GetTutorialQuery, Result<TutorialResponse>>
{
    public async Task<Result<TutorialResponse>> Handle(GetTutorialQuery request, CancellationToken cancellationToken)
    {
        var tutorial = await repository.TutorialsRepository.GetTutorialById(request.TutorialId, cancellationToken);

        if (tutorial == null)
        {
            var error = TutorialErrors.TutorialNotFound(request.TutorialId);
            
            log.Warning(error.Detail);
            return Result<TutorialResponse>.Failure(error);
        }
        
        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        var user = await repository.UsersRepository.GetUserByIdAsync(tutorial.CreatedBy, cancellationToken);
        var userResponse = mapper.Map<UserSummaryResponse>(user);
            
        userDictionary.Add(tutorial.TutorialId, userResponse);
        
        var tutorialResponse = tutorialMapper.Map([tutorial], userDictionary).First();
        
        log.Information($"Found tutorial with id {tutorial.TutorialId}");

        return Result<TutorialResponse>.Success(tutorialResponse);
    }
}