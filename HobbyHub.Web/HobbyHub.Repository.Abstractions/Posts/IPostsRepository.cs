using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Posts;

public interface IPostsRepository
{
    Task Add(Post post, CancellationToken cancellationToken);
    Task<List<Post>> GetPostsForUser(int userId, CancellationToken cancellationToken);
}