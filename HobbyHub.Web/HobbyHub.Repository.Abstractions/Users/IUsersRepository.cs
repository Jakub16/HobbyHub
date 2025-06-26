using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Users;

public interface IUsersRepository
{
    Task Add(User user, CancellationToken cancellationToken);
    Task<int> Follow(int userId, int userToUnfollowId, CancellationToken cancellationToken);
    Task<int> Unfollow(int userId, int userToUnfollowId, CancellationToken cancellationToken);
    Task<bool> IsFollowing(int userId, int followedUserId, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);
    User? GetUserById(int userId);
    Task<List<User>> GetUsersByIds(List<int> userIds, CancellationToken cancellationToken);
    Task<bool> UserExists(int userId, CancellationToken cancellationToken);
    Task<List<User>> GetUserFollows(int userId, CancellationToken cancellationToken);
    Task<List<User>> UsersSearch(string keyword, CancellationToken cancellationToken);
}