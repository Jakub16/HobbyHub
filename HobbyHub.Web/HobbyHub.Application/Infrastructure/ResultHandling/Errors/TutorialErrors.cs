using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public class TutorialErrors
{
    public static Error TutorialNotFound(int tutorialId) => new Error(
        "Not Found", $"Tutorial with id {tutorialId} not found", StatusCodes.Status404NotFound);
    public static Error TutorialNotFoundCommand(int tutorialId) => new Error(
        "Not Found", $"Tutorial with id {tutorialId} not found", StatusCodes.Status400BadRequest);
    public static Error TutorialStepNotFoundCommand(int tutorialStepId) => new Error(
        "Not Found", $"Tutorial step with id {tutorialStepId} not found", StatusCodes.Status400BadRequest);
}