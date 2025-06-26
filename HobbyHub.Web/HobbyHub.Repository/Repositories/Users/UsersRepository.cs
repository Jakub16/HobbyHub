using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Users;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Users;

public class UsersRepository(AppDbContext dbContext) : IUsersRepository
{
    public async Task Add(User user, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> Follow(int userId, int userToUnfollowId, CancellationToken cancellationToken)
    {
        var follow = new Follow()
        {
            FollowerId = userId,
            FollowedId = userToUnfollowId
        };

        await dbContext.Follows.AddAsync(follow, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return follow.FollowId;
    }

    public async Task<int> Unfollow(int userId, int userToUnfollowId , CancellationToken cancellationToken)
    {
        var follow = await dbContext.Follows
            .Include(follow => follow.Follower)
            .Include(follow => follow.Followed)
            .FirstOrDefaultAsync(follow =>
                follow.Follower.UserId == userId && follow.Followed.UserId == userToUnfollowId, cancellationToken);

        dbContext.Follows.Remove(follow);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return follow.FollowId;
    }

    public async Task<bool> IsFollowing(int userId, int followedUserId, CancellationToken cancellationToken)
    {
        return await dbContext.Follows
            .AsNoTracking()
            .Include(follow => follow.Followed)
            .Include(follow => follow.Follower)
            .AnyAsync(follow => 
                follow.Follower.UserId == userId && follow.Followed.UserId == followedUserId, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);
    }

    public User? GetUserById(int userId)
    {
        return dbContext.Users.FirstOrDefault(user => user.UserId == userId);
    }

    public async Task<List<User>> GetUsersByIds(List<int> userIds, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => userIds.Contains(user.UserId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users.AsNoTracking().AnyAsync(user => user.UserId == userId, cancellationToken);
    }

    public async Task<List<User>> GetUserFollows(int userId, CancellationToken cancellationToken)
    {
        var follows = await dbContext.Follows
            .Include(follow => follow.Followed)
            .Where(follow => follow.FollowerId == userId)
            .ToListAsync(cancellationToken);

        return follows.Select(follow => follow.Followed).ToList();
    }

    public async Task<List<User>> UsersSearch(string keyword, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => EF.Functions.Like(user.Email.ToLower(), $"%{keyword.ToLower()}%")
                           || EF.Functions.Like(user.Name.ToLower(), $"%{keyword.ToLower()}%")
                           || EF.Functions.Like(user.Surname.ToLower(), $"%{keyword.ToLower()}%"))
            .ToListAsync(cancellationToken);
    }
}