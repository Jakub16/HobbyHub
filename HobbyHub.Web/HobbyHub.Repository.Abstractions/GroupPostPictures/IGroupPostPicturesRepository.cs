using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.GroupPostPictures;

public interface IGroupPostPicturesRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(GroupPostPicture groupPostPicture, CancellationToken cancellationToken);
    Task<GroupPostPicture> GetGroupPostPictureById(int groupPostPictureId, CancellationToken cancellationToken);
}