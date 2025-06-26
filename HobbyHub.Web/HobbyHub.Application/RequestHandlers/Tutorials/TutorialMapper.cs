using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;

namespace HobbyHub.Application.RequestHandlers.Tutorials;

public interface ITutorialMapper
{
    List<TutorialResponse> Map(List<Tutorial> tutorials,
        Dictionary<int, UserSummaryResponse> userDictionary);
}

public class TutorialMapper : ITutorialMapper
{
    public List<TutorialResponse> Map(List<Tutorial> tutorials, Dictionary<int, UserSummaryResponse> userDictionary)
    {
        return tutorials.Select(tutorial =>
        {
            return new TutorialResponse()
            {
                TutorialId = tutorial.TutorialId,
                Title = tutorial.Title,
                Description = tutorial.Description,
                Category = tutorial.Category,
                Attachments = tutorial.Attachments,
                Resources = tutorial.Resources,
                Price = tutorial.Price,
                MainPicturePath = tutorial.MainPicturePath,
                CreatedBy = userDictionary[tutorial.TutorialId],
                TutorialSteps = tutorial.TutorialSteps.Select(tutorialStep =>
                {
                    return new TutorialStepResponse()
                    {
                        TutorialStepId = tutorialStep.TutorialStepId,
                        Title = tutorialStep.Title,
                        Content = tutorialStep.Content,
                        StepNumber = tutorialStep.StepNumber,
                        PathsToPictures = tutorialStep.TutorialStepPictures
                            .Select(x => x.PathToPicture)
                            .ToList()
                    };
                }).ToList()
            };
        }).ToList();
    }
}