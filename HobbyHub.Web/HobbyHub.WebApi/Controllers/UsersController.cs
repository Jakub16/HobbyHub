using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Activities.GetUserActiveActivity;
using HobbyHub.Application.RequestHandlers.Activities.GetUserEndedActivities;
using HobbyHub.Application.RequestHandlers.Events.GetUserEvents;
using HobbyHub.Application.RequestHandlers.Feed.GetFeedPosts;
using HobbyHub.Application.RequestHandlers.Groups.GetFriendsGroups;
using HobbyHub.Application.RequestHandlers.Groups.GetSuggestedGroups;
using HobbyHub.Application.RequestHandlers.Groups.GetUserGroups;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;
using HobbyHub.Application.RequestHandlers.Marketplace.GetSavedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Marketplace.GetUserCreatedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Marketplace.GetUserOrderedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Tutorials.GetUserAssignedTutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.GetUserTutorials;
using HobbyHub.Application.RequestHandlers.Users.CreateOrUpdateProfilePicture;
using HobbyHub.Application.RequestHandlers.Users.FollowUser;
using HobbyHub.Application.RequestHandlers.Users.GetUserFollows;
using HobbyHub.Application.RequestHandlers.Users.GetUserSummary;
using HobbyHub.Application.RequestHandlers.Users.SearchUsers;
using HobbyHub.Application.RequestHandlers.Users.UnfollowUser;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;
using HobbyHub.Contract.Responses.Activities;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Feed;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Users Controller
/// </summary>
[Produces("application/json")]
[Authorize]
public class UsersController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Follows new user.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("user/follow")]
    public async Task<ActionResult<CommandResponse>> Follow([FromBody] FollowUserRequest request)
    {
        var result = await requestHandler.HandleRequest(new FollowUserCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Unfollows user.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("user/unfollow")]
    public async Task<ActionResult<CommandResponse>> Unfollow([FromBody] UnfollowUserRequest request)
    {
        var result = await requestHandler.HandleRequest(new UnfollowUserCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's follows.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/follows")]
    public async Task<ActionResult<ListResponse<UserSummaryResponse>>> Follows(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserFollowsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Searches users by keyword.
    /// </summary>
    /// <param name="keyword"></param>
    [HttpGet]
    [Route("usersSearch")]
    public async Task<ActionResult<ListResponse<UserSummaryResponse>>> UsersSearch(string keyword)
    {
        var result = await requestHandler.HandleRequest(new SearchUsersQuery(keyword));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's events.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/events")] //TODO change to user/{userId:int}/events
    public async Task<ActionResult<ListResponse<EventResponse>>> UserEvents(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserEventsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's saved marketplace items.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/savedMarketPlaceItems")]
    public async Task<ActionResult<ListResponse<MarketplaceItemResponse>>> SavedMarketplaceItems(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetSavedMarketplaceItemsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's created marketplace items.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/marketplaceItems")]
    public async Task<ActionResult<ListResponse<MarketplaceItemResponse>>> CreatedMarketplaceItems(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserCreatedMarketplaceItemsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's ordered marketplace items.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/marketplaceItems/ordered")]
    public async Task<ActionResult<ListResponse<MarketplaceItemResponse>>> OrderedMarketplaceItems(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserOrderedMarketplaceItemsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's summary.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/summary")]
    public async Task<ActionResult<UserSummaryResponse>> UserSummary(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserSummaryQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's created tutorials.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/tutorials")]
    public async Task<ActionResult<ListResponse<TutorialResponse>>> UserTutorials(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserTutorialsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's assigned tutorials.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/tutorials/assigned")]
    public async Task<ActionResult<ListResponse<TutorialResponse>>> UserAssignedTutorials(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserAssignedTutorialsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's active activity.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/activities/active")]
    public async Task<ActionResult<ActivityResponse?>> ActiveActivity(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserActiveActivityQuery(userId));
        
        if (!result.IsSuccess && result.Error?.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's ended activitiews.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/activities/ended")]
    public async Task<ActionResult<ListResponse<ActivityResponse>>> EndedActivities(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserEndedActivitiesQuery(userId));
        
        if (!result.IsSuccess && result.Error?.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's feed posts.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/feed")]
    public async Task<ActionResult<ListResponse<FeedPostResponse>>> Feed(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetFeedPostsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's groups.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet]
    [Route("user/{userId:int}/groups")]
    public async Task<ActionResult<ListResponse<GroupSummaryResponse>>> UserGroups(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserGroupsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's (custom) hobbies.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/userHobbies")]
    public async Task<ActionResult<ListResponse<HobbyResponse>>> UserHobbies(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetUserHobbiesQuery(userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's favorite (custom and predefined) hobbies.
    /// </summary>
    /// <remarks>
    ///     Predefined hobbies have IsHobbyPredefined field set to true
    ///
    ///     Custom hobbies have IsHobbyPredefined field set to false
    ///
    ///     Both of these entities have independent ids
    /// </remarks>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/favoriteHobbies")]
    public async Task<ActionResult<ListResponse<FavoriteHobbyResponse>>> FavoriteHobbies(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetFavoriteHobbiesQuery(userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes user hobby with the provided id and deletes corresponding favorite hobby if it exists.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userHobbyId"></param>
    [HttpDelete, Route("user/{userId:int}/userHobby/{userHobbyId:int}")]
    public async Task<ActionResult<CommandResponse>> DeleteFavoriteHobby(int userId, int userHobbyId)
    {
        var result = await requestHandler.HandleRequest(new DeleteUserHobbyCommand(userId, userHobbyId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's suggested groups.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/suggestedGroups")]
    public async Task<ActionResult<ListResponse<GroupSummaryResponse>>> SuggestedGroups(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetSuggestedGroupsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets user's friends groups.
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet, Route("user/{userId:int}/friendsGroups")]
    public async Task<ActionResult<ListResponse<GroupSummaryResponse>>> FriendsGroups(int userId)
    {
        var result = await requestHandler.HandleRequest(new GetFriendsGroupsQuery(userId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Creates or updates user's profile picture.
    /// </summary>
    /// <param name="userId"></param>
    [HttpPut, Route("user/{userId:int}/profilePicture")]
    public async Task<ActionResult<CommandResponse>> UserProfilePicture(int userId, [FromForm] CreateOrUpdateProfilePictureRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateOrUpdateProfilePictureCommand(userId, request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
}