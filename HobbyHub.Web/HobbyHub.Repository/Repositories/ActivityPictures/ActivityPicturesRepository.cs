using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.ActivityPictures;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.ActivityPictures;

public class ActivityPicturesRepository(AppDbContext dbContext) : IActivityPicturesRepository
{
    public async Task Add(ActivityPicture activityPicture, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(activityPicture, cancellationToken);
    }

    public async Task<ActivityPicture> GetActivityPictureById(int activityPictureId, CancellationToken cancellationToken)
    {
        return await dbContext.ActivityPictures.SingleAsync(picture => picture.ActivityPhotoId == activityPictureId,
            cancellationToken);
    }
}