using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.ActivityPictures;

public interface IActivityPicturesRepository
{
    Task Add(ActivityPicture activityPicture, CancellationToken cancellationToken);
    Task<ActivityPicture> GetActivityPictureById(int activityPictureId, CancellationToken cancellationToken);
}