using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Events.AddEventMember;
using HobbyHub.Application.RequestHandlers.Events.CreateEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEvent;
using HobbyHub.Application.RequestHandlers.Events.DeleteEventMember;
using HobbyHub.Application.RequestHandlers.Events.GetEvent;
using HobbyHub.Application.RequestHandlers.Events.GetEventMembers;
using HobbyHub.Application.RequestHandlers.Events.UpdateEvent;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Events Controller
/// </summary>
[Authorize]
public class EventsController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Creates event.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("event")]
    public async Task<ActionResult<CommandResponse>> CreateEvent([FromForm] CreateEventRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateEventCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Updates event.
    /// </summary>
    /// <param name="request"></param>
    [HttpPut]
    [Route("event")]
    public async Task<ActionResult<CommandResponse>> UpdateEvent([FromForm] UpdateEventRequest request)
    {
        var result = await requestHandler.HandleRequest(new UpdateEventCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Adds new user to the event.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="userId"></param>
    [HttpPut]
    [Route("event/{eventId:int}/user/{userId:int}")]
    public async Task<ActionResult<CommandResponse>> AddEventMember(int eventId, int userId)
    {
        var result = await requestHandler.HandleRequest(new AddEventMemberCommand(eventId, userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }
    
    /// <summary>
    /// Deletes user from the event.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="userId"></param>
    [HttpDelete]
    [Route("event/{eventId:int}/user/{userId:int}")]
    public async Task<ActionResult<CommandResponse>> DeleteEventMember(int eventId, int userId)
    {
        var result = await requestHandler.HandleRequest(new DeleteEventMemberCommand(eventId, userId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="eventId"></param>
    [HttpDelete]
    [Route("event/{eventId:int}")]
    public async Task<ActionResult<CommandResponse>> DeleteEvent(int eventId)
    {
        var result = await requestHandler.HandleRequest(new DeleteEventCommand(eventId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets event.
    /// </summary>
    /// <param name="eventId"></param>
    [HttpGet]
    [Route("event/{eventId:int}")]
    public async Task<ActionResult<EventResponse>> GetEvent(int eventId)
    {
        var result = await requestHandler.HandleRequest(new GetEventQuery(eventId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets event members.
    /// </summary>
    /// <param name="eventId"></param>
    [HttpGet]
    [Route("event/{eventId:int}/users")]
    public async Task<ActionResult<ListResponse<UserSummaryResponse>>> GetEventMembers(int eventId)
    {
        var result = await requestHandler.HandleRequest(new GetEventMembersQuery(eventId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
}