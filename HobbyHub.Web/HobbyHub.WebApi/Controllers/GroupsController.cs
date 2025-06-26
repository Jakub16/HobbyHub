using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Events.AddEventMember;
using HobbyHub.Application.RequestHandlers.Events.CreateEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEventMember;
using HobbyHub.Application.RequestHandlers.Events.GetGroupEvents;
using HobbyHub.Application.RequestHandlers.Events.UpdateEvent;
using HobbyHub.Application.RequestHandlers.Groups.AddGroupMember;
using HobbyHub.Application.RequestHandlers.Groups.CreateGroup;
using HobbyHub.Application.RequestHandlers.Groups.CreateGroupPost;
using HobbyHub.Application.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;
using HobbyHub.Application.RequestHandlers.Groups.DeleteGroupMember;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupPosts;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupSummary;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;
using HobbyHub.Application.RequestHandlers.Groups.GetPublicGroups;
using HobbyHub.Application.RequestHandlers.Groups.GetUserGroups;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Groups Controller
/// </summary>
[Produces("application/json")]
[Authorize]
public class GroupsController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("group")]
    public async Task<ActionResult<CommandResponse>> CreateGroup([FromForm] CreateGroupRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateGroupCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Adds a new member to the existing group.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="userId"></param>
    [HttpPut]
    [Route("group/{groupId:int}/user/{userId:int}")]
    public async Task<IActionResult> AddGroupMember(int groupId, int userId)
    {
        var result = await requestHandler.HandleRequest(new AddGroupMemberCommand(groupId, userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }
    
    /// <summary>
    /// Deletes a member from the group.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="userId"></param>
    [HttpDelete]
    [Route("group/{groupId:int}/user/{userId:int}")]
    public async Task<ActionResult<CommandResponse>> DeleteMember(int groupId, int userId)
    {
        var result = await requestHandler.HandleRequest(new DeleteGroupMemberCommand(groupId, userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets group members.
    /// </summary>
    /// <param name="groupId"></param>
    [HttpGet]
    [Route("group/{groupId:int}/users")]
    public async Task<ActionResult<ListResponse<UserSummaryResponse>>> GetGroupUsers(int groupId)
    {
        var result = await requestHandler.HandleRequest(new GetGroupUsersQuery(groupId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets group summary.
    /// </summary>
    /// <param name="groupId"></param>
    [HttpGet]
    [Route("group/{groupId:int}")]
    public async Task<ActionResult<GroupSummaryResponse>> GetGroupSummary(int groupId)
    {
        var result = await requestHandler.HandleRequest(new GetGroupSummaryQuery(groupId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Creates group post.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("group/post")]
    public async Task<ActionResult<CommandResponse>> CreateGroupPost([FromForm] CreateGroupPostRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateGroupPostCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets group posts.
    /// </summary>
    /// <param name="groupId"></param>
    [HttpGet]
    [Route("group/{groupId:int}/posts")]
    public async Task<ActionResult<ListResponse<GroupPostResponse>>> GetGroupPosts(int groupId)
    {
        var result = await requestHandler.HandleRequest(new GetGroupPostsQuery(groupId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets group's events.
    /// </summary>
    /// <param name="groupId"></param>
    [HttpGet]
    [Route("group/{groupId:int}/events")]
    public async Task<ActionResult<ListResponse<EventResponse>>> GetGroupEvents(int groupId)
    {
        var result = await requestHandler.HandleRequest(new GetGroupEventsQuery(groupId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Creates or update group's main picture.
    /// </summary>
    /// <param name="request"></param>
    [HttpPut]
    [Route("group/mainPicture")]
    public async Task<ActionResult<CommandResponse>> MainPicture([FromForm] CreateOrUpdateGroupMainPictureRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateOrUpdateGroupMainPictureCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets public groups.
    /// </summary>
    /// <param name="groupId"></param>
    [HttpGet]
    [Route("groups/public")]
    public async Task<ActionResult<ListResponse<GroupSummaryResponse>>> PublicGroups()
    {
        var result = await requestHandler.HandleRequest(new GetPublicGroupsQuery());
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
}