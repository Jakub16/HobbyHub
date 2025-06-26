using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Groups;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Groups;

public class GroupsRepository(AppDbContext dbContext) : IGroupsRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(Group group, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(group, cancellationToken);
    }

    public async Task<bool> GroupExists(int groupId, CancellationToken cancellationToken)
    {
        return await dbContext.Groups.AsNoTracking().AnyAsync(group => group.GroupId == groupId, cancellationToken);
    }

    public async Task<bool> IsUserGroupMember(int groupId, User user, CancellationToken cancellationToken)
    {
        var group = await GetGroupById(groupId, cancellationToken);

        return group.Users.Contains(user);
    }

    public async Task<Group?> GetGroupById(int groupId, CancellationToken cancellationToken)
    {
        return await dbContext.Groups
            .Include(group => group.Users)
            .FirstOrDefaultAsync(group => group.GroupId == groupId, cancellationToken);
    }

    public async Task<List<Group>> GetUserGroups(int userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);
        
        return await dbContext.Groups
            .AsNoTracking()
            .Include(group => group.Users)
            .Where(group => user != null && group.Users.Contains(user))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetGroupMembers(int groupId, CancellationToken cancellationToken)
    {
        var group = await dbContext.Groups
            .AsNoTracking()
            .Include(x => x.Users)
            .FirstOrDefaultAsync(group => group.GroupId == groupId, cancellationToken);

        return group?.Users;
    }

    public async Task<List<Group>> GetPublicGroups(CancellationToken cancellationToken)
    {
        return await dbContext.Groups
            .AsNoTracking()
            .Include(group => group.Users)
            .Where(group => !group.IsPrivate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Group>> 
        GetSuggestedGroups(int userId, List<string> keywords, CancellationToken cancellationToken)
    {
        var query = dbContext.Groups
            .AsNoTracking()
            .AsQueryable()
            .Where(group => !group.IsPrivate);
        
        keywords = keywords.Select(keyword => keyword.ToLower()).ToList();
            
        query = query.Where(group => keywords.Any(keyword =>
            EF.Functions.Like(group.Name.ToLower(), keyword)
            || (group.Description != null && EF.Functions.Like(group.Description.ToLower(), keyword))));

        return await query
            .Include(group => group.Users)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Group>> GetFriendsGroups(int userId, CancellationToken cancellationToken)
    {
        var followsId = await dbContext.Follows
            .AsNoTracking()
            .Where(follow => follow.FollowerId == userId)
            .Select(follow => follow.FollowedId)
            .ToListAsync(cancellationToken);
        
        return await dbContext.Groups
            .AsNoTracking()
            .Include(group => group.Users)
            .Where(group => !group.IsPrivate)
            .Where(group => followsId.Contains(group.CreatedBy))
            .ToListAsync(cancellationToken);
    }
}