using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Groups;

public interface IGroupsRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(Group group, CancellationToken cancellationToken);
    Task<bool> GroupExists(int groupId, CancellationToken cancellationToken);
    Task<bool> IsUserGroupMember(int groupId, User user, CancellationToken cancellationToken);
    Task<Group?> GetGroupById(int groupId, CancellationToken cancellationToken);
    Task<List<Group>> GetUserGroups(int userId, CancellationToken cancellationToken);
    Task<List<User>> GetGroupMembers(int groupId, CancellationToken cancellationToken);
    Task<List<Group>> GetPublicGroups(CancellationToken cancellationToken);
    Task<List<Group>> GetSuggestedGroups(int userId, List<string> keywords, CancellationToken cancellationToken);
    Task<List<Group>> GetFriendsGroups(int userId, CancellationToken cancellationToken);
}