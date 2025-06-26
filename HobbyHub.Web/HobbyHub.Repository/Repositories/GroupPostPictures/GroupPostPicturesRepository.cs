using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.GroupPostPictures;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.GroupPostPictures;

public class GroupPostPicturesRepository(AppDbContext dbContext) : IGroupPostPicturesRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(GroupPostPicture groupPostPicture, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(groupPostPicture, cancellationToken);
    }

    public async Task<GroupPostPicture> GetGroupPostPictureById(int groupPostPictureId, CancellationToken cancellationToken)
    {
        return await dbContext.GroupPostPictures.SingleAsync(groupPostPicture =>
            groupPostPicture.GroupPostPictureId == groupPostPictureId, cancellationToken);
    }
}