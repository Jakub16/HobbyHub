using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Tutorials;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorialStep;

public class CreateTutorialStepCommand(CreateTutorialStepRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int TutorialId { get; } = request.TutorialId;
    public string Title { get; } = request.Title;
    public string Content { get; } = request.Content;
    public int StepNumber { get; } = request.StepNumber;
    public List<IFormFile>? Pictures { get; } = request.Pictures;
}