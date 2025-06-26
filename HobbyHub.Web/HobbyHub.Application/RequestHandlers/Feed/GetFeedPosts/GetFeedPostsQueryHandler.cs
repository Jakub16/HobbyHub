using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Feed;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Feed.GetFeedPosts;

public class GetFeedPostsQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetFeedPostsQuery, Result<ListResponse<FeedPostResponse>>>
{
    public async Task<Result<ListResponse<FeedPostResponse>>> Handle(GetFeedPostsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<FeedPostResponse>>.Failure(error);
        }

        var posts = await repository.PostsRepository.GetPostsForUser(request.UserId, cancellationToken);

        var result = posts.Select(post =>
        {
            var isActivityEnded = post.Activity.EndTime != null;

            var hobby = post.Activity.Hobby;
            var userHobby = post.Activity.UserHobby;

            var hobbyResponse = hobby != null
                ? mapper.Map<HobbyResponse>(hobby)
                : mapper.Map<HobbyResponse>(userHobby);

            return new FeedPostResponse()
            {
                ActivityId = post.Activity.ActivityId,
                Content = post.Activity.Notes.FirstOrDefault()?.Content ?? string.Empty,
                DateTime = isActivityEnded ? post.Activity.EndTime : post.Activity.StartTime,
                Duration = post.Activity.Time,
                Hobby = hobbyResponse,
                Distance = post.Activity.Distance,
                IsDistanceAvailable = post.Activity.IsDistanceAvailable,
                IsActivityEnded = isActivityEnded,
                PathsToPictures = post.Activity.Pictures.Select(picture => picture.PathToPicture).ToList(),
                UserSummary = mapper.Map<UserSummaryResponse>(post.Activity.User)
            };
        }).ToList();
        
        log.Information($"Found {result.Count} feed posts for user with id {request.UserId}");
        
        return Result<ListResponse<FeedPostResponse>>.Success(new ListResponse<FeedPostResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}