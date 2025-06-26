using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Posts;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Posts;

public class PostsRepository(AppDbContext dbContext) : IPostsRepository
{
    public async Task Add(Post post, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(post, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Post>> GetPostsForUser(int userId, CancellationToken cancellationToken)
    {
        var follows = await dbContext.Follows
            .AsNoTracking()
            .Include(follow => follow.Followed)
            .Where(follow => follow.FollowerId == userId)
            .ToListAsync(cancellationToken);

        var followingUsersIds = follows.Select(follow => follow.Followed.UserId);

        return await dbContext.Posts
            .AsNoTracking()
            .Include(post => post.Activity)
            .ThenInclude(activity => activity.User)
            .Include(post => post.Activity)
            .ThenInclude(activity => activity.Notes)
            .Include(post => post.Activity)
            .ThenInclude(activity => activity.Pictures)
            .Include(post => post.Activity)
            .ThenInclude(activity => activity.Hobby)
            .Include(post => post.Activity)
            .ThenInclude(activity => activity.UserHobby)
            .Where(post => post.Activity.User.UserId == userId || followingUsersIds.Contains(post.Activity.User.UserId))
            .OrderByDescending(post => post.Activity.EndTime ?? post.Activity.StartTime)
            .ToListAsync(cancellationToken);
    }
}