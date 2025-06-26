using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.TutorialStepPictures;

public interface ITutorialStepPicturesRepository
{
    TutorialStepPicture GetTutorialStepPictureById(int tutorialStepPictureId);
}