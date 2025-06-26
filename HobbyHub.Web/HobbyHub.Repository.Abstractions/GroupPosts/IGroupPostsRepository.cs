using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.GroupPosts;

public interface IGroupPostsRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(GroupPost groupPost, CancellationToken cancellationToken);
    Task<List<GroupPost>> GetGroupPosts(int groupId, CancellationToken cancellationToken);
}