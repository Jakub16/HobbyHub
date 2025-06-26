using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.GroupPosts;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.GroupPosts;

public class GroupPostsRepository(AppDbContext dbContext) : IGroupPostsRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(GroupPost groupPost, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(groupPost, cancellationToken);
    }

    public async Task<List<GroupPost>> GetGroupPosts(int groupId, CancellationToken cancellationToken)
    {
        var group = await dbContext.Groups
            .Include(x => x.Users)
            .Include(x => x.GroupPosts)
            .ThenInclude(groupPost => groupPost.GroupPostPictures)
            .FirstOrDefaultAsync(group => group.GroupId == groupId, cancellationToken);

        return group?.GroupPosts.OrderByDescending(groupPost => groupPost.GroupPostId).ToList() ?? [];
    }
}