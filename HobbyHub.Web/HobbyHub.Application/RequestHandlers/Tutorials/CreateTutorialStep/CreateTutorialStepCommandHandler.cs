using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorialStep;

public class CreateTutorialStepCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateTutorialStepCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateTutorialStepCommand request, CancellationToken cancellationToken)
    {
        var tutorial = await repository.TutorialsRepository.GetTutorialById(request.TutorialId, cancellationToken);

        if (tutorial == null)
        {
            var error = TutorialErrors.TutorialNotFoundCommand(request.TutorialId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var tutorialStep = new TutorialStep()
        {
            Content = request.Content,
            StepNumber = request.StepNumber,
            Title = request.Title,
            Tutorial = tutorial
        };
            
        await repository.Add(tutorialStep, cancellationToken);
        await repository.SaveChanges(cancellationToken);
            
        request.Pictures?.ForEach(picture => AddPicture(picture).GetAwaiter().GetResult());

        log.Information($"Created new tutorial step with id {tutorialStep.TutorialStepId} with {request.Pictures?.Count} picture(s)");
        
        return Result<CommandResponse>.Success(new CommandResponse(tutorialStep.TutorialStepId));

        async Task AddPicture(IFormFile requestPicture)
        {
            var tutorialStepPicture = new TutorialStepPicture()
            {
                TutorialStep = tutorialStep
            };

            await repository.Add(tutorialStepPicture, CancellationToken.None);
            await repository.SaveChanges(CancellationToken.None);
            
            var pictureUrl = await fileUploader.Send(
                "tutorials/steps",
                $"tutorial_{tutorial.TutorialId.ToString()}/step_{tutorialStep.TutorialStepId.ToString()}/tutorialStepPicture_{tutorialStepPicture.TutorialStepPictureId}",
                requestPicture);

            var foundTutorialStepPicture =
                repository.TutorialStepPicturesRepository.GetTutorialStepPictureById(tutorialStepPicture.TutorialStepPictureId);

            foundTutorialStepPicture.PathToPicture = pictureUrl;
            await repository.SaveChanges(CancellationToken.None);
        }
    }
}