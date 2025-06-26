using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Feed;

namespace HobbyHub.Application.RequestHandlers.Feed.GetFeedPosts;

public class GetFeedPostsQuery(int userId) : IHobbyHubRequest<Result<ListResponse<FeedPostResponse>>>
{
    public int UserId { get; } = userId;
}