using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.TutorialStepPictures;

namespace HobbyHub.Repository.Repositories.TutorialStepPictures;

public class TutorialStepPicturesRepository(AppDbContext dbContext) : ITutorialStepPicturesRepository
{
    public TutorialStepPicture GetTutorialStepPictureById(int tutorialStepPictureId)
    {
        return dbContext.TutorialStepPictures.Single(tutorialStepPicture =>
            tutorialStepPicture.TutorialStepPictureId == tutorialStepPictureId);
    }
}